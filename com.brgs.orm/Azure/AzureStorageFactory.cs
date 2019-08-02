using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using com.brgs.orm.Azure.helpers;
using System;

namespace com.brgs.orm.Azure
{
    public class AzureStorageFactory: IAzureStorage
    {
        private ICloudStorageAccount account;
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            account = acc;

        }
        public void Register(string collection, string partition)
        {


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
            
            var dict = new Dictionary<string,string>(){
                {"CollectionName", CollectionName},
                {"PartitionKey", PartitionKey}
            };
            new AzureTableBuilder(account, dict).PostAsync(record).GetAwaiter().GetResult();
            return record;
        }
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records)
        {
            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }
            if(string.IsNullOrEmpty("PartitionKey"))
            {
                throw new ArgumentException("we need to hava a partition Key");
            }
            var dict = new Dictionary<string,string>(){
                {"CollectionName", CollectionName},
                {"PartitionKey", PartitionKey}
            };
            return await new AzureTableBuilder(account, dict).PostBatchAsync(records, "");
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