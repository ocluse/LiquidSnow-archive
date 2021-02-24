using System;
using System.IO;
using System.Threading;
using Thismaker.Core; 

namespace Thismaker.Aba.Client.Transfers
{
    public class StreamTransfer : Transfer
    {
        /// <summary>
        /// The stream for the transfer, the destination or the source, must be a stream with random read access
        /// </summary>
        public Stream Stream { get; internal set; }

        public StreamTransfer() : base()
        {

        }
    }

    public class FileTransfer : Transfer
    {
        /// <summary>
        /// The path of the transfer, where it will be saved, or from where it will be uploaded
        /// </summary>
        public string Path { get; internal set; }

        public FileTransfer() : base()
        {

        }
    }

    public abstract class Transfer : BindableBase
    {
        #region Private Fields
        private string _name, _blobName;
        private TransferMode _mode;
        private TransferState _state;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion

        #region Init

        public event Action<Transfer> TransferCancelled;

        internal Transfer()
        {
            State = TransferState.Waiting;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the transfer, as a general identifier
        /// </summary>
        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        /// <summary>
        /// The name of the blob associated with th transfer
        /// </summary>
        public string BlobName
        {
            get { return _blobName; }
            internal set { SetProperty(ref _blobName, value); }
        }

        /// <summary>
        /// Whether the transfer is an upload operation or a download opertaion
        /// </summary>
        public TransferMode Mode
        {
            get { return _mode; }
            internal set { SetProperty(ref _mode, value); }
        }

        /// <summary>
        /// The state of the transfer, this will be set by the TransferManager
        /// </summary>
        public TransferState State
        {
            get { return _state; }
            internal set { SetProperty(ref _state, value); }
        }

        /// <summary>
        /// The cancellation token associated with the transfer, 
        /// that will be tripped when the transfer is cancelled.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancels the transfer, if it had already started or waiting in a queue
        /// </summary>
        public void Cancel()
        {
            if(State!=TransferState.Cancelled)
            _cancellationTokenSource.Cancel();
            State = TransferState.Cancelled;
            TransferCancelled.Invoke(this);
        }
        #endregion
    }

    public enum TransferState
    {
        Processing, Waiting, Error, Requeued, Cancelled
    }

    public enum TransferMode
    {
        Upload, Download
    }
}
