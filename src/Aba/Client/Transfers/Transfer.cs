using System.IO;
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
        internal Transfer()
        {
            State = TransferState.Waiting;
        }

        private string name, blobName;
        private TransferMode mode;
        private TransferState state;

        /// <summary>
        /// The name of the transfer, as a general identifier
        /// </summary>
        public string Name
        {
            get { return name; }
            internal set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// The name of the blob associated with th transfer
        /// </summary>
        public string BlobName
        {
            get { return blobName; }
            internal set { SetProperty(ref blobName, value); }
        }

        /// <summary>
        /// Whether the transfer is an upload operation or a download opertaion
        /// </summary>
        public TransferMode Mode
        {
            get { return mode; }
            internal set { SetProperty(ref mode, value); }
        }

        /// <summary>
        /// The state of the transfer, this will be set by the TransferManager
        /// </summary>
        public TransferState State
        {
            get { return state; }
            internal set { SetProperty(ref state, value); }
        }
    }

    public enum TransferState
    {
        Processing, Waiting, Error, Requeued
    }

    public enum TransferMode
    {
        Upload, Download
    }
}
