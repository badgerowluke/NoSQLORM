using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using com.brgs.orm.Azure.helpers;
using System;

namespace com.brgs.orm.Azure
{
    public class AzureStorageFactory: AzureFormatHelper, IAzureStorage
    {
        private ICloudStorageAccount account;
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
            return await new AzureTableBuilder(account, CollectionName).PostBatchAsync(records, "");
        }   
        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public int Delete<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }             
    }
}