using System.IO;

namespace Thismaker.Mercury.Azure.Blobs
{
    public static class TransferBuilder
    {
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
