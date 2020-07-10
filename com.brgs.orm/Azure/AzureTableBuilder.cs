
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage.Table;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]


namespace com.brgs.orm.Azure
{
    public interface IAzureTableBuilder
    {
        Task<T> GetAsync<T>(TableQuery query);
        Task<T> GetAsync<T>(TableQuery query, string collection);
        Task<int> PostBatchAsync<T>(IEnumerable<T> records);
        Task<string> PostStorageTableAsync<T>(T value);
        Task DeleteBatchAsync<T>(IEnumerable<T> records);
    }
    public partial class AzureStorageFactory: IAzureTableBuilder
    {

        public async Task<T> GetAsync<T>(TableQuery query, string collection)
        {
           return await InternalGetAsync<T>(query, collection);  
        }

        public async Task<T> GetAsync<T>(TableQuery query) 
        {
            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }            
            return await InternalGetAsync<T>(query, CollectionName);
        }

        public virtual async Task<IEnumerable<T>> GetFromStorageTableAsync<T>(Expression<Func<T,bool>> predicate)
        {
            
            var query = new TableQuery().Where(BuildQueryFilter(predicate));

            return await InternalGetAsync<List<T>>(query, CollectionName);

        }        


        protected async Task<T> InternalGetAsync<T>(TableQuery query, string collection)
        {

            var table = _tableclient.GetTableReference(collection);
            var tableExists = await table.ExistsAsync();

            if(!tableExists)
                return default(T);

                
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

        public async Task<string> PostStorageTableAsync<T>(T value)
        {
            
            var table = _tableclient.GetTableReference(CollectionName);
            await table.CreateIfNotExistsAsync();

            TableOperation insert = null;
            if(value is ITableEntity)
            {
                if(string.IsNullOrEmpty((value as ITableEntity).PartitionKey))
                {
                    (value as ITableEntity).PartitionKey = PartitionKey;
                }                    
                insert = TableOperation.InsertOrMerge((ITableEntity) value);

            } 
            else 
            {
                var obj = BuildTableEntity(value);
                insert = TableOperation.InsertOrMerge((ITableEntity) obj);
            }
            var val =  await table.ExecuteAsync(insert);
            return val.HttpStatusCode.ToString();      
        }        

        ///<summary>each individual batch needs to be less than or equal to 100</summary>
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
        {

            var table = _tableclient.GetTableReference(CollectionName);

            await table.CreateIfNotExistsAsync();

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
                    await table.ExecuteBatchAsync(batch);
                    recordCount = recordCount + partial.Count();

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

            await Task.WhenAll(tasks).ConfigureAwait(false);            


        } 
    }
}