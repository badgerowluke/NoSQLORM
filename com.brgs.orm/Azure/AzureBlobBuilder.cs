using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

using com.brgs.orm.Azure.helpers;

namespace com.brgs.orm.Azure
{
    internal class AzureBlobBuilder
    {
        private ICloudStorageAccount account;

        private AzureFormatHelper helpers { get; set; }

        public AzureBlobBuilder(ICloudStorageAccount acc)
        {
            account = acc;
            helpers = new AzureFormatHelper();
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
    }
}