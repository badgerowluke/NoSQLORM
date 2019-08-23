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

namespace com.brgs.orm.Azure
{
    public abstract class AzureStorageFactory: AzureFormatHelper, IAzureStorage
    {
        protected ICloudStorageAccount _account;
        protected CloudTableClient _tableclient { get; set; }
        protected CloudBlobClient _blobClient { get; set; }
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
        public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate, string collection = null)
        {
            var query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), new FeedOptions
                {
                    EnableCrossPartitionQuery = true
                })
                .Where(predicate)
                .AsDocumentQuery();
            
            var results = new List<T>();
            while(query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }        
      
        public virtual async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate)
        {
            var query = new TableQuery().Where(BuildQueryFilter(predicate));
            
            return await InternalGetAsync<IEnumerable<T>>(query, CollectionName);
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
            return new AzureTableBuilder(_account)
                                .GetAsync<T>(query, CollectionName).Result;
        }
        public virtual async Task<int> PostAsync<T>(T record)
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
                return 1;


            } catch (StorageException e )
            {
                throw new Exception(e.RequestInformation.ExtendedErrorInformation.ToString());
            }        
        }
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
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
        public virtual void DeleteBatchAsync<T>(IEnumerable<T> records)
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

            IList<TableResult>[] results =  Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();            


        }              
        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public Task DeleteAsync<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }

        public async Task DeleteAsync<T>(string id, string partiton = null)
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
        public async void DeleteBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey)
        {
            await _client.ExecuteStoredProcedureAsync<T>(UriFactory.CreateStoredProcedureUri(DatabaseId,CollectionId, procName),
            new RequestOptions()
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(partitionKey)
            }, records);            
            
        }        
    }
}