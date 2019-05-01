using System;
using System.Collections.Generic;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
>>>>>>> Stashed changes
using System.Threading.Tasks;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure
{
    internal class AzureTableBuilder
    {
        private ICloudStorageAccount account { get; set; }
        private AzureFormatHelper helpers { get; set; }
        private string Collection { get; set; }
        public AzureTableBuilder(ICloudStorageAccount acc)
        {
            account = acc;
            helpers = new AzureFormatHelper();
        }
        public AzureTableBuilder(ICloudStorageAccount acc, string collection)
        {
            account = acc;
            helpers = new AzureFormatHelper();
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
                        var val =  helpers.RecastEntity(entity, content);
                        outVal.GetType().GetMethod("Add").Invoke(outVal, new object[] { val });
                    }
                    else
                    { 
                        return (T)helpers.RecastEntity(entity, typeof(T));
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
        public async Task<IList<TableResult>> PostBatchAsync<T>(IEnumerable<T> records)
        {
            TableBatchOperation batch = new TableBatchOperation();

            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Collection);
            var results = await table.ExecuteBatchAsync(batch);

            return results;
        }
    }
}