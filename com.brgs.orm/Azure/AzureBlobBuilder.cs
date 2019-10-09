using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

using com.brgs.orm.Azure.helpers;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq.Expressions;

namespace com.brgs.orm.Azure
{
    public interface IAzureBlobBuilder
    {
        Task<T> GetAsync<T>(string containerName);
    }
    
    public class AzureBlobBuilder : AzureStorageFactory
    {


        public AzureBlobBuilder(ICloudStorageAccount acc)
        {
            _account = acc;
            _blobClient = _account.CreateCloudBlobClient();

        }

        public override async Task<T> GetAsync<T>(string blobName)
        {
            return await base.GetAsync<T>(blobName);
        }

        public override async Task<string> PostAsync<T>(T record)
        {
            int count = 0;
            var container = _blobClient.GetContainerReference(CollectionName);
            var blob = container.GetBlockBlobReference(PartitionKey);
            var vals = JsonConvert.SerializeObject(record);
            var bytes = Encoding.UTF8.GetBytes(vals);
            using(var stream = new MemoryStream(bytes))
            {
                await blob.UploadFromStreamAsync(stream);
                count++;
            }
            return count.ToString();

        }
    }
}