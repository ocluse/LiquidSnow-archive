using System.IO;

namespace Thismaker.Aba.Client.Transfers
{
    public class TransferBuilder<T> where T:Transfer, new()
    {

        readonly T transfer;

        public TransferBuilder()
        {
            transfer = new T();
        }

        public TransferBuilder<T> WithName(string name)
        {
            transfer.Name = name;
            return this;
        }

        public TransferBuilder<T> WithMode(TransferMode mode)
        {
            transfer.Mode = mode;
            return this;
        }

        public TransferBuilder<T> WithBlobName(string name)
        {
            transfer.BlobName = name;
            return this;
        }

        public TransferBuilder<T> WithLocal(string path)
        {
            ((FileTransfer)(object)transfer).Path = path;
            return this;
        }
        
        public TransferBuilder<T> WithLocal(Stream stream)
        {
            ((StreamTransfer)(object)transfer).Stream = stream;
            return this;
        }

        public T Build()
        {
            return transfer;
        }

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
