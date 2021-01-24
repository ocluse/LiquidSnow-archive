using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferManager
    {
        #region Private Fields
        private readonly Queue<Transfer> pending;
        private readonly ITransferAuthenticator auth;
        private readonly Timer transferTimer;
        #endregion

        #region Events
        /// <summary>
        /// Fired whenever an error occurs with the transfer processs for <b>queued</b> Transfers
        /// </summary>
        public event Action<Exception> TransferFailed;
        #endregion

        #region Properties


        /// <summary>
        /// A list of the current transfers, including the inactive ones
        /// </summary>
        public ObservableCollection<Transfer> ActiveTransfers { get; private set; }
        #endregion

        #region Initialization
        public TransferManager(ITransferAuthenticator authenticator)
        {
            //Set authenticator:
            auth = authenticator;

            //Init:
            ActiveTransfers = new ObservableCollection<Transfer>();
            pending = new Queue<Transfer>();

            //Transfer timer:
            transferTimer = new Timer(1.0f)
            {
                Enabled = true,
                AutoReset = true
            };
            transferTimer.Elapsed += TransferTimer_Elapsed;
            transferTimer.Start();
        }

        private async void TransferTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Confirm that we have files currently pending
            if (pending.Count == 0)
            {
                return;
            }
            //Disable the transfer timer
            transferTimer.Enabled = false;

            //Get the next transfer in the queue
            var transfer = pending.Dequeue();

            //Wait for the transfer
            await Transfer(transfer, true).ContinueWith(t => {
                TransferFailed?.Invoke(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);

            //Enable the transfer timer
            transferTimer.Enabled = true;

        }
        #endregion

        #region Private Methods
        private async Task Transfer(Transfer transfer, bool requeueOnError)
        {
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
                    await blob.UploadAsync(stream, true);
                }
                else
                {
                    if (!blob.Exists().Value)
                    {
                        throw new NullReferenceException("Blob not found");
                    }
                    await blob.DownloadToAsync(stream);
                }

                stream.Dispose();
            }
            catch (DirectoryNotFoundException ex)
            {
                transfer.State = TransferState.Error;
                string message = $"The file: '{transfer.Name}' destined for upload could not be located in the physical disk. The directory does not exist";
                throw new TransferManagerException(message, ex);
                //Processor.Log(, LogLevel.Error);
            }
            catch (FileNotFoundException ex)
            {
                transfer.State = TransferState.Error;
                string message = $"The file: '{transfer.Name}' destined for upload could not be located in the physical disk. The file does not exists";
                throw new TransferManagerException(message, ex);
                //Processor.Log(, LogLevel.Error);
            }
            catch (IOException ex)
            {
                //Processor.Log(, LogLevel.Error);

                //Requeue on error
                if (requeueOnError)
                {
                    transfer.State = TransferState.Requeued;
                    pending.Enqueue(transfer);
                }
                string message = $"The file: '{transfer.Name}' is still being used by another application/process, it has been requeued and will be uploaded once it's available";
                throw new TransferManagerException(message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                string message = $"Spectre does not have permision to access: '{transfer.Name}.' It will not be uploaded";
                transfer.State = TransferState.Requeued;
                //Processor.Log(, LogLevel.Error);
                if (requeueOnError)
                {
                    transfer.State = TransferState.Requeued;
                    pending.Enqueue(transfer);
                }

                throw new TransferManagerException(message, ex);
                //Requeue on error
            }

            catch (Exception ex)
            {
                string message = $"Failed to upload the file: '{transfer.Name}' due to unknown reasons";
                transfer.State = TransferState.Error;
                //Processor.Log(, LogLevel.Error);
                throw ex;
            }

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a transfer to the transfer queue, recommended for load balancing.
        /// Any errors or exceptions are reported through the OnTransferExceptionEvent rather than via rethrow.
        /// </summary>
        /// <param name="transfer"></param>
        public void Enqueue(Transfer transfer)
        {
            transfer.State = TransferState.Waiting;
            if (ActiveTransfers.Any(x => x.BlobName == transfer.BlobName && x.Mode == transfer.Mode))
            {
                throw new InvalidOperationException("Transfer already exists in queue");
                //Processor.Log($"The transfer {transfer.Name} already exists in the queue and was not readded", LogLevel.Info);
            }

            pending.Enqueue(transfer);
            ActiveTransfers.Add(transfer);
        }

        /// <summary>
        /// Immediately begins a transfer, rather enqueuing it for processing at it's turn, 
        /// any error that occurs is directly rethrown to the calling thread
        /// </summary>
        /// <param name="transfer">The transfer to work with</param>
        /// <returns></returns>
        public async Task Invoke(Transfer transfer)
        {
            //Add it to the ActiveTransfers list for notification
            ActiveTransfers.Add(transfer);

            try
            {
                //Start the task:
                await Transfer(transfer, false);
            }
            catch
            {
                throw;
            }
            finally
            {
                //Attempt to remove from active transfers, in case it has not been removed yet
                ActiveTransfers.Remove(transfer);
            }
        }
        #endregion
    }
}
