using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.brgs.orm.Azure
{
    internal class AzureQueueBuilder
    {
        private ICloudStorageAccount _account { get; set; }
        private CloudQueueClient _client;
        public AzureQueueBuilder(ICloudStorageAccount account)
        {
            _account  = account;
            _client = _account.CreateCloudQueueClient();
        }
        public string Post<T>(T value, string container)
        {
            var queue = _client.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var obj = JsonConvert.SerializeObject(value);
            var message = new CloudQueueMessage(obj);
            
            queue.AddMessageAsync(message).GetAwaiter().GetResult();
            return queue.ApproximateMessageCount.ToString();
        }
        public T Get<T>(string container)
        {
            var queue = _client.GetQueueReference(container);
            var message = queue.GetMessageAsync().GetAwaiter().GetResult();
            if(message != null)
            {

                queue.DeleteMessageAsync(message).GetAwaiter().GetResult();
                var jObject = (JObject) JsonConvert.DeserializeObject(message.AsString); 
                return jObject.ToObject<T>();
            }
            return default(T);
        }
        public string Peek(string container)
        {
            var queue = _client.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var peekedMessage = queue.PeekMessageAsync().GetAwaiter().GetResult();
            if(peekedMessage != null)
            {
                return peekedMessage.AsString; 
            }
            return string.Empty;
        }
        public async Task<int> GetApproximateQueueMessageCount(string container)
        {          
            var queue = _client.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            await queue.FetchAttributesAsync();
            return queue.ApproximateMessageCount != null ? (int)queue.ApproximateMessageCount : 0;
        }        
    }
}