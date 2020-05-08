using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

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

        public override async Task<T> GetAsync<T>(string fileName)
        {
            return await base.GetAsync<T>(fileName);
        }

        public override async Task<string> PostAsync<T>(T value)
        {
            int count = 0;
            var container = _blobClient.GetContainerReference(CollectionName);
            var blob = container.GetBlockBlobReference(PartitionKey);
            var vals = JsonConvert.SerializeObject(value);
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