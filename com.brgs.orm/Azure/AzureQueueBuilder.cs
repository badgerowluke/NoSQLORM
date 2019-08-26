using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.brgs.orm.Azure
{
    public interface IAzureQueueBuilder 
    {
        string Post<T>(T value, string container);
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

        
        public override async Task<T> GetAsync<T>(string container)
        {
            var queue = _queueClient.GetQueueReference(container);
            var message = await queue.GetMessageAsync();//.GetAwaiter().GetResult();
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