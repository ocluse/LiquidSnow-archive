using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Thismaker.Core.Models;
using System.Collections.Specialized;
using System.Collections;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferManager : INotifyCollectionChanged, IEnumerable<Transfer>
    {
        #region Implementations
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<Transfer> GetEnumerator()
        {
            var active = new List<Transfer>();
            active.AddRange(_queued);
            active.AddRange(_active);

            return active.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var active = new List<Transfer>();
            active.AddRange(_queued);
            active.AddRange(_active);

            return active.GetEnumerator();
        }

        #endregion

        #region Private Fields
        private readonly ObservableCollection<Transfer> _active;
        private readonly ObservableQueue<Transfer> _queued;
        private readonly ITransferAuthenticator auth;
        #endregion

        #region Events
        /// <summary>
        /// Fired whenever an error occurs with the transfer processs for <b>queued</b> Transfers
        /// </summary>
        public event Action<Exception> TransferFailed;
        
        #endregion

        #region Properties

        public bool RequeueOnError
        {
            get;set;
        }
        #endregion

        #region Initialization
        public TransferManager(ITransferAuthenticator authenticator)
        {
            //Set authenticator:
            auth = authenticator;

            _queued = new ObservableQueue<Transfer>();

            _queued.CollectionChanged += OnQueuedChanged;
            _active.CollectionChanged += OnInvokedChanged;
        }

        private void OnInvokedChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged.Invoke(this, e);
        }

        private void OnQueuedChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Where an item is added, try to do our work:
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (_queued.Count == 1)
                {
                    try
                    {
                        DequeueTransfer();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            //Inform our observers:
            CollectionChanged.Invoke(this, e);
        }

        #endregion

        #region Private Methods
        private async void DequeueTransfer()
        {
            //Get the next transfer in the queue
            var transfer = _queued.Dequeue();

            //Wait for the transfer
            try
            {
                await Transfer(transfer);
            }
            catch (TransferException ex)
            {
                if (ex.InnerException.GetType() == typeof(IOException)
                   && RequeueOnError
                   && ex.Message.EndsWith("is being used by another process"))
                {
                    transfer.State = TransferState.Requeued;
                    _queued.Enqueue(transfer);
                }

                TransferFailed.Invoke(ex);
            }
            catch (Exception)
            {
                throw;
            }

            if (_queued.Count > 0)
            {
                DequeueTransfer();
            }
        }

        private async Task Transfer(Transfer transfer)
        {
            //The Cancelled Transfer should not be executed
            if (transfer.State == TransferState.Cancelled)
            {
                return;
            }
            _active.Add(transfer);
            try
            {
                //Get the SAS for the transfer
                var uri = await auth.GetSASToken(transfer.BlobName);

                //build the blob:
                var blob = new BlobClient(uri);
                Stream stream;

                //Locate the destination/origin stream for the transfer
                if (transfer.GetType() == typeof(StreamTransfer))
                {
                    stream = ((StreamTransfer)transfer).Stream;
                }
                else
                {
                    if (transfer.Mode == TransferMode.Upload)
                    {
                        stream = File.OpenRead(((FileTransfer)transfer).Path);
                    }
                    else
                    {
                        stream = File.OpenWrite(((FileTransfer)transfer).Path);
                    }

                }

                //Upload or download depending on the transfer
                if (transfer.Mode == TransferMode.Upload)
                { 
                    await blob.UploadAsync(stream, true, transfer.CancellationToken);
                }
                else
                {
                    if (!blob.Exists().Value)
                    {
                        throw new TransferException("Blob not found");
                    }
                    
                    await blob.DownloadToAsync(stream, transfer.CancellationToken);
                }

                stream.Dispose();
            }
            catch (DirectoryNotFoundException ex)
            {
                transfer.State = TransferState.Error;
                string message = $"Directory for the transfer '{transfer.Name}' does not exist";
                throw new TransferException(message, ex);
            }
            catch (FileNotFoundException ex)
            {
                transfer.State = TransferState.Error;
                string message = $"The file for transfer '{transfer.Name}' was not found";
                throw new TransferException(message, ex);
            }
            catch (IOException ex)
            {
                string message = $"The file for transfer '{transfer.Name}' is being used by another process";
                throw new TransferException(message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                string message = $"Insufficient permission to access the file for tranfser '{transfer.Name}.'";
                transfer.State = TransferState.Requeued;
                throw new TransferException(message, ex);
            }
            finally
            {
                _active.Remove(transfer);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a transfer to the queue, recommended for load balancing.
        /// Transfer related exceptions are reported through the <see cref="TransferFailed"/> event.
        /// Non-related errors will be thrown to the calling thread.
        /// </summary>
        /// <param name="transfer">the transfer to add to queue</param>
        /// <param name="requeue">If true, the transfer will be requeued even if it's already in the queue.</param>
        /// <returns>True if the item was successfully added to queue</returns>
        public bool Enqueue(Transfer transfer, bool requeue=false)
        {
            transfer.State = TransferState.Waiting;
            if (_queued.Any(x => x.BlobName == transfer.BlobName && x.Mode == transfer.Mode))
            {
                if (!requeue) return false;
            }
            _queued.Enqueue(transfer);

            
            return true;
        }

        private void OnTransferCancelled(Transfer sender)
        {
            if (_active.Contains(sender))
            {
                _active.Remove(sender);
            }
        }

        /// <summary>
        /// Immediately begins a transfer, rather than waiting for all active transfers to be completed.
        /// Any error that occurs is directly rethrown to the calling thread
        /// </summary>
        /// <param name="transfer">The transfer to execute</param>
        /// <returns></returns>
        public async Task Invoke(Transfer transfer)
        {
            try
            {
                transfer.TransferCancelled += OnTransferCancelled;
                //Start the task:
                await Transfer(transfer);
                
            }
            catch
            {
                throw;
            }
            finally
            {
                transfer.TransferCancelled -= OnTransferCancelled;
            }
        }

        #endregion
    }
}
