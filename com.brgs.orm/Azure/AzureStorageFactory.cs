using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using com.brgs.orm.Azure.helpers;

namespace com.brgs.orm.Azure
{
    public class AzureStorageFactory: AzureFormatHelper, IStorageFactory
    {
        private ICloudStorageAccount account;
        public string CollectionName { get; set; }


        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            account = acc;

        }
        public T Get<T>( string blobName)
        {
            return new AzureBlobBuilder(account)
                                .GetAsync<T>(CollectionName, blobName).Result;
        }

        public T Get<T>(TableQuery query) 
        {
            return new AzureTableBuilder(account)
                                .GetAsync<T>(query, CollectionName).Result;
        }
        public T Post<T>(T record)
        {
            var tableRunner = new AzureTableBuilder(account, CollectionName);
            if(record is ITableEntity)
            {
                //just send the object in
                var table = tableRunner.PostAsync(record).Result;
            }
            else
            {
                PartitionKey = "";

                var obj = BuildTableEntity(record);
                
                TableResult table = tableRunner.PostAsync((ITableEntity) obj).Result;
            }
            return record;
        }
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
        {
            
            TableBatchOperation batch = new TableBatchOperation();
            IList<TableResult> result = null;
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(CollectionName);
            await table.CreateIfNotExistsAsync();
            foreach(var record in records)
            {
                if(record is ITableEntity)
                {
                    batch.Insert((ITableEntity)record);
                } else 
                {
                    var obj = BuildTableEntity(record);
                    batch.Insert((ITableEntity) obj);
                }
            }


            result = await table.ExecuteBatchAsync(batch);
            
            return result.Count;
            
        }        
    }
}