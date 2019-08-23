using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

using com.brgs.orm.Azure.helpers;
using System.Collections.Generic;
using System.Text;

namespace com.brgs.orm.Azure
{
    internal class AzureBlobBuilder : AzureFormatHelper
    {
        private ICloudStorageAccount account;


        public AzureBlobBuilder(ICloudStorageAccount acc)
        {
            account = acc;

        }
        public async Task<T> GetAsync<T>(string containerName, string blobName)
        {
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(blobName);
            var blobStream = await blob.OpenReadAsync();
            using(StreamReader reader = new StreamReader(blobStream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public async Task<int> PostAsync<T>(T record)
        {
            int count = 0;
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(CollectionName);
            var blob = container.GetBlockBlobReference(PartitionKey);
            var vals = JsonConvert.SerializeObject(record);
            var bytes = Encoding.UTF8.GetBytes(vals);
            using(var stream = new MemoryStream(bytes))
            {
                await blob.UploadFromStreamAsync(stream);
                count++;
            }
            return count;

        }
    }
}