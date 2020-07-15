using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.brgs.orm.Azure
{
    public interface IAzureQueueBuilder 
    {
        Task<string> PostQueueMessageAsync<T>(T value, string container);
        Task<T> GetQueueMessageAsync<T>(string fileName);
        string Peek(string container);
        Task<int> GetApproximateQueueMessageCount(string container);
    }
    public partial class  AzureStorageFactory : IAzureQueueBuilder
    {


        public async Task<T> GetQueueMessageAsync<T>(string fileName)
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

        ///<summary>
        ///<param name="Value">The object to be converted to a queue message</param>
        ///<param name="container">The Queue to post into</param>
        ///</summary>
        public async Task<string> PostQueueMessageAsync<T>(T value, string container)
        {
            var queue = _queueClient.GetQueueReference(container);

            var obj = JsonConvert.SerializeObject(value);
            var message = new CloudQueueMessage(obj);
            await queue.AddMessageAsync(message);
            await queue.FetchAttributesAsync();


            
            return queue.ApproximateMessageCount.ToString();
        }   

        public virtual string Peek(string container)
        {
            var queue = _queueClient.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var peekedMessage = queue.PeekMessageAsync().GetAwaiter().GetResult();
            if(peekedMessage != null)
            {
                return peekedMessage.AsString; 
            }
            return string.Empty;
        }  
        /* this is currently untestable because I am unaware how to set the readonly property here. */
        public virtual async Task<int> GetApproximateQueueMessageCount(string container)
        {          
            var queue = _queueClient.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            await queue.FetchAttributesAsync();
            return queue.ApproximateMessageCount ?? 0;
        }    
    }
}