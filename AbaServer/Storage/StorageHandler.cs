using Azure.Storage;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Aba.Server.Storage
{
    public abstract class StorageHandler
    {
        public string AccountName { get; set; }

        public string ContainerName { get; set; }

        public string AccountKey { get; set;}

        private BlobServiceClient serviceClient;

        private BlobContainerClient containerClient;

        public virtual void Initialize()
        {
            var blobEndpoint = $"https://{AccountName}.blob.core.windows.net";
            var cred = new StorageSharedKeyCredential(AccountName, AccountKey);
            serviceClient = new BlobServiceClient(new Uri(blobEndpoint), cred);
            containerClient = serviceClient.GetBlobContainerClient(ContainerName);
        }

        public bool BlobExists(string name)
        {
            var bc = containerClient.GetBlobClient(name);
            try
            {
                var exists = bc.Exists();
                return exists.Value;
            }
            catch
            {
                throw;
            }
        }

        public void GetBlob(string name, Stream destination)
        {
            var bc = containerClient.GetBlobClient(name);

            if (!bc.Exists().Value)
            {
                throw new InvalidOperationException("Blob does not exist");
            }

            bc.DownloadTo(destination);
        }

        public async Task GetBlobAsync(string name, Stream destination)
        {
            var bc = containerClient.GetBlobClient(name);

            if (!bc.Exists().Value)
            {
                throw new InvalidOperationException("Blob does not exist");
            }

            await bc.DownloadToAsync(destination);
        }

        public void SetBlob(string name, Stream source)
        {
            var bc = containerClient.GetBlobClient(name);

            source.Position = 0;
            bc.Upload(source, true);
        }

        public async Task SetBlobAsync(string name, Stream source)
        {
            var bc = containerClient.GetBlobClient(name);

            source.Position = 0;
            await bc.UploadAsync(source, true);
        }
    }
}
