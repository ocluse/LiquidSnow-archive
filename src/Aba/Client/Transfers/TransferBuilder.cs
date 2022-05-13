using System.IO;

namespace Thismaker.Aba.Client.Transfers
{
    /// <summary>
    /// Create a transfer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TransferBuilder<T> where T:Transfer, new()
    {

        readonly T transfer;
        /// <summary>
        /// Initialize a new instance of the builder.
        /// </summary>
        public TransferBuilder()
        {
            transfer = new T();
        }
        /// <summary>
        /// Set the name of the transfer
        /// </summary>
        public TransferBuilder<T> WithName(string name)
        {
            transfer.Name = name;
            return this;
        }
        /// <summary>
        /// Set the mode of the transfer
        /// </summary>
        public TransferBuilder<T> WithMode(TransferMode mode)
        {
            transfer.Mode = mode;
            return this;
        }
        /// <summary>
        /// Set the name of the blob.
        /// </summary>
        public TransferBuilder<T> WithBlobName(string name)
        {
            transfer.BlobName = name;
            return this;
        }
        /// <summary>
        /// Set the uri of the blob.
        /// </summary>
        public TransferBuilder<T> WithBlobUri(string uri)
        {
            transfer.BlobUri = uri;
            return this;
        }
        /// <summary>
        /// The local path of the transfer
        /// </summary>
        public TransferBuilder<T> WithLocal(string path)
        {
            ((FileTransfer)(object)transfer).Path = path;
            return this;
        }
        /// <summary>
        /// The local stream of the transfer
        /// </summary>
        public TransferBuilder<T> WithLocal(Stream stream)
        {
            ((StreamTransfer)(object)transfer).Stream = stream;
            return this;
        }
        /// <summary>
        /// Create the transfer
        /// </summary>
        public T Build()
        {
            return transfer;
        }
        /// <summary>
        /// Creates a transfer to upload a file from a path
        /// </summary>
        public static FileTransfer CreateFileUpload(string transferName, string blobName, string path)
        {
            return new FileTransfer
            {
                Mode = TransferMode.Upload,
                Name = transferName,
                Path = path,
                BlobName = blobName
            };
        }
        /// <summary>
        /// Creates a transfer to download a file
        /// </summary>
        public static FileTransfer CreateFileDownload(string transferName, string blobName, string path)
        {
            return new FileTransfer
            {
                Mode = TransferMode.Upload,
                Name = transferName,
                Path = path,
                BlobName = blobName,
            };
        }
        /// <summary>
        /// Creates a transfer to upload a stream
        /// </summary>
        public static StreamTransfer CreateStreamUpload(string transferName, string blobName, Stream stream)
        {
            return new StreamTransfer
            {
                Mode = TransferMode.Upload,
                Name = transferName,
                Stream = stream,
                BlobName = blobName,
            };
        }
        /// <summary>
        /// Creates a transfer to upload a stream.
        /// </summary>
        public static StreamTransfer CreateStreamDownload(string transferName, string blobName, Stream stream)
        {
            return new StreamTransfer
            {
                Mode = TransferMode.Upload,
                Name = transferName,
                Stream = stream,
                BlobName = blobName,
            };
        }
    }
}
