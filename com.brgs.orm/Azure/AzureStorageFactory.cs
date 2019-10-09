using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using com.brgs.orm.Azure.helpers;
using System;
using System.Linq.Expressions;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.Storage.Queue;

namespace com.brgs.orm.Azure
{
    public abstract class AzureStorageFactory: AzureFormatHelper, IAzureStorage
    {
        protected ICloudStorageAccount _account;
        protected CloudTableClient _tableclient { get; set; }
        protected CloudBlobClient _blobClient { get; set; }
        protected CloudQueueClient _queueClient { get; set; }
        protected IDocumentClient _client { get; set; }
        protected string DatabaseId { get;  set; }
        protected string CollectionId { get; set; }

        protected string _containerName;

        public virtual async Task<T> GetAsync<T>( string fileName)
        {
            var blobClient = _account.CreateCloudBlobClient();

            var container = _blobClient.GetContainerReference(_containerName);
            var blob = container.GetBlobReference(fileName);
            var blobStream = await blob.OpenReadAsync();
            using(StreamReader reader = new StreamReader(blobStream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
       
      
        public virtual async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate)
        {
            try
            {
                if(string.IsNullOrEmpty(CollectionName)) { throw new ArgumentNullException("Collection cannot be null"); }
                var query = new TableQuery().Where(BuildQueryFilter(predicate));

                /* TODO Fix the IEnumerable issue cropping up now. */
                return await InternalGetAsync<List<T>>(query, CollectionName);

            } catch (Exception e)
            {
                throw e;
            }
        }
        protected async Task<T> InternalGetAsync<T>(TableQuery query, string collection)
        {
            var table = _tableclient.GetTableReference(collection);
            var outVal = (T)Activator.CreateInstance(typeof(T));
            var content = outVal.GetType().GetGenericArguments().Length > 0 ? outVal.GetType().GetGenericArguments()[0] : null ;            
            TableContinuationToken token = null;
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(query, token);
                token = results.ContinuationToken;
                foreach(var entity in results.Results)
                {
                    if (outVal.GetType().GetMethod("Add") != null && content != null)
                    {
                        var val =  RecastEntity(entity, content);
                        outVal.GetType().GetMethod("Add").Invoke(outVal, new object[] { val });
                    }
                    else
                    { 
                        return (T)RecastEntity(entity, typeof(T));
                    }
                }

            } while (token != null);

            return outVal; 
        }        

        public T Get<T>(TableQuery query) 
        {
            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }            
            return InternalGetAsync<T>(query, CollectionName).GetAwaiter().GetResult();
        }
        ///<summary>

        ///<param name="Value">The object to be converted to a queue message</param>
        ///<param name="container">The Queue to post into</param>
        ///</summary>
        public virtual string Post<T>(T value, string container)
        {
            var queue = _queueClient.GetQueueReference(container);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var obj = JsonConvert.SerializeObject(value);
            var message = new CloudQueueMessage(obj);
            queue.AddMessageAsync(message).GetAwaiter().GetResult();
            queue.FetchAttributesAsync().GetAwaiter().GetResult();
            
            return queue.ApproximateMessageCount.ToString();
        }        
        public virtual async Task<string> PostAsync<T>(T record)
        {
            
            try 
            {                
                var table = _tableclient.GetTableReference(CollectionName);
                bool complete = table.CreateIfNotExistsAsync().Result;
                TableOperation insert = null;
                if(record is ITableEntity)
                {
                    if(string.IsNullOrEmpty((record as ITableEntity).PartitionKey))
                    {
                        (record as ITableEntity).PartitionKey = PartitionKey;
                    }                    
                    insert = TableOperation.InsertOrMerge((ITableEntity) record);

                } 
                else 
                {
                    var obj = BuildTableEntity(record);
                    insert = TableOperation.InsertOrMerge((ITableEntity) obj);
                }
                var val =  await table.ExecuteAsync(insert);
                return val.HttpStatusCode.ToString();


            } catch (StorageException e )
            {
                throw new Exception(e.RequestInformation.ExtendedErrorInformation.ToString());
            }        
        }

        public virtual async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
        {
            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }
            if(string.IsNullOrEmpty(PartitionKey))
            {
                throw new ArgumentException("we need to hava a partition Key");
            }

            var table = _tableclient.GetTableReference(CollectionName);

            var didCreate = await table.CreateIfNotExistsAsync();

            IList<TableResult> result = null;

            if (records.Count() <= 100)
            {
                var batch = BuildBatch<T>(records, PartitionKey);
                result = await table.ExecuteBatchAsync(batch);
                return result.Count;
            }
            else
            {
                int recordCount = 0;
                do
                {
                    var partial = records.Skip(recordCount).Take(100);
                    var batch = BuildBatch(partial, PartitionKey);
                    result = await table.ExecuteBatchAsync(batch);
                    var val = partial.Count();
                    recordCount = recordCount + partial.Count();;

                } while (recordCount < records.Count());
                return recordCount;
            }
        }   
        private TableBatchOperation BuildBatch<T>(IEnumerable<T> records, string partition)
        {
            TableBatchOperation batch = new TableBatchOperation();
            foreach (var record in records)
            {
                if (record is ITableEntity)
                {
                    if(string.IsNullOrEmpty((record as ITableEntity).PartitionKey))
                    {
                        (record as ITableEntity).PartitionKey = partition;
                    }
                    batch.InsertOrReplace((ITableEntity)record);
                }
                else
                {
                    var obj = BuildTableEntity(record);
                    batch.InsertOrReplace((ITableEntity)obj);
                }
            }
            return batch;
        }  
        public virtual async Task DeleteBatchAsync<T>(IEnumerable<T> records)
        {         
            var table = _tableclient.GetTableReference(CollectionName);
            Action<TableBatchOperation, ITableEntity> batchOperationAction = null;
            batchOperationAction = (bo, entity) => bo.Delete(entity);
            TableBatchOperation batch = new TableBatchOperation();
            var tasks = new List<Task<IList<TableResult>>>();
            var entitiesOffset = 0;
            while (entitiesOffset < records?.Count())
            {
                var entitiesToAdd = records.Skip(entitiesOffset).Take(100).ToList();
                entitiesOffset += entitiesToAdd.Count;

                TableBatchOperation batchOperation = new TableBatchOperation();

                entitiesToAdd.ForEach(entity => batchOperationAction(batchOperation, (ITableEntity)entity));

                tasks.Add(table.ExecuteBatchAsync(batchOperation));
            }

            IList<TableResult>[] results =  await Task.WhenAll(tasks).ConfigureAwait(false);            


        }              
        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public Task DeleteAsync<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }

        public virtual async Task DeleteAsync<T>(string id, string partiton = null)
        {
            if(partiton == null)
            {

                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions
                {
                    PartitionKey = new PartitionKey(Undefined.Value)
                });          
            }


            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions
            {
                PartitionKey = new PartitionKey(partiton)
            });          

        }
        public virtual async void DeleteBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey)
        {
            await _client.ExecuteStoredProcedureAsync<T>(UriFactory.CreateStoredProcedureUri(DatabaseId,CollectionId, procName),
            new RequestOptions()
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(partitionKey)
            }, records);            
            
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