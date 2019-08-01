using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure
{
    internal class AzureTableBuilder: AzureFormatHelper
    {
        private ICloudStorageAccount account { get; set; }

        private string Collection { get; set; }
        public AzureTableBuilder(ICloudStorageAccount acc)
        {
            account = acc;

        }
        public AzureTableBuilder(ICloudStorageAccount acc, string collection)
        {
            account = acc;

            Collection = collection;
            
        }
        public async Task<T> GetAsync<T>(TableQuery query, string collection)
        {
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(collection);
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
        public async Task<TableResult> PostAsync<T>(T record)
        {
            try 
            {                
                var tableClient = account.CreateCloudTableClient();
                var table = tableClient.GetTableReference(Collection);
                bool complete = table.CreateIfNotExistsAsync().Result;
                var insert = TableOperation.InsertOrMerge((ITableEntity) record);

                var val =  await table.ExecuteAsync(insert);
                return val;

            } catch (StorageException e )
            {
                throw new Exception(e.RequestInformation.ExtendedErrorInformation.ToString());
            }            
        }
        ///<summary>each individual batch needs to be less than or equal to 100</summary>
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
        {

            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Collection);

            var didCreate = await table.CreateIfNotExistsAsync();

            IList<TableResult> result = null;

            if (records.Count() <= 100)
            {
                var batch = BuildBatch<T>(records);
                result = await table.ExecuteBatchAsync(batch);
                return result.Count;
            }
            else
            {
                int recordCount = 0;
                do
                {
                    var partial = records.Skip(recordCount).Take(100);
                    var batch = BuildBatch(partial);
                    result = await table.ExecuteBatchAsync(batch);
                    var val = partial.Count();
                    recordCount = recordCount + partial.Count();;

                } while (recordCount < records.Count());
                return recordCount;
            }
        }

        private TableBatchOperation BuildBatch<T>(IEnumerable<T> records)
        {
            TableBatchOperation batch = new TableBatchOperation();
            foreach (var record in records)
            {
                if (record is ITableEntity)
                {
                    if(string.IsNullOrEmpty((record as ITableEntity).PartitionKey))
                    {
                        (record as ITableEntity).PartitionKey = to.PartitionKey;
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
        public void DeleteBatchAsync<T>(IEnumerable<T> records)
        {

            var tableClient = account.CreateCloudTableClient();            
            var table = tableClient.GetTableReference(to.CollectionName);
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
    }
}