using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using com.brgs.orm.Azure.helpers;
using System;
using System.Linq.Expressions;

namespace com.brgs.orm.Azure
{
    public class AzureStorageFactory: IAzureStorage
    {
        private ICloudStorageAccount account;
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        private readonly Interegator _parser;
        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            account = acc;
            _parser = new Interegator();

        }

        public T Get<T>( string blobName)
        {
            return new AzureBlobBuilder(account)
                                .GetAsync<T>(CollectionName, blobName).Result;
        }
        public IEnumerable<T> Get<T>(Expression<Func<T,bool>> predicate)
        {

            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }
            var query = new TableQuery();
            var filter = _parser.BuildQueryFilter(predicate);
            query.Where(filter);

            return new AzureTableBuilder(account)
                                .GetAsync<List<T>>(query, CollectionName).GetAwaiter().GetResult();
        }

        public T Get<T>(TableQuery query) 
        {
            if(string.IsNullOrEmpty(CollectionName))
            {
                throw new ArgumentException("we need to have a collection");
            }            
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