using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.brgs.orm.Azure
{
    public interface IAzureQueueBuilder 
    {
        Task<string> Post<T>(T value, string container);
        Task<T> GetAsync<T>(string container);
        string Peek(string container);
        Task<int> GetApproximateQueueMessageCount(string container);
    }
    public class AzureQueueBuilder : AzureStorageFactory, IAzureQueueBuilder
    {
        public AzureQueueBuilder(ICloudStorageAccount account)
        {
            _account  = account;
            _queueClient = _account.CreateCloudQueueClient();
        }

        public override async Task<T> GetAsync<T>(string fileName)
        {
            var queue = _queueClient.GetQueueReference(fileName);
            var message = await queue.GetMessageAsync();
            if(message != null)
            {

                await queue.DeleteMessageAsync(message);
                var jObject = (JObject) JsonConvert.DeserializeObject(message.AsString); 
                return jObject.ToObject<T>();
            }
            return default(T);
        }    
    }
}