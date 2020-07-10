using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace com.brgs.orm.Azure
{
    public interface IAzureBlobBuilder
    {
        Task<T> GetAsync<T>(string containerName);
        Task<string> PostAsync<T>(T value);
    }
    
    public partial class  AzureStorageFactory: IAzureBlobBuilder 
    {

        public async Task<T> GetAsync<T>(string fileName)
        {
            var container = _blobClient.GetContainerReference(_containerName);
            var blob = container.GetBlobReference(fileName);
            var blobStream = await blob.OpenReadAsync();
            using(StreamReader reader = new StreamReader(blobStream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public async Task<string> PostBlobAsync<T>(T value)
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