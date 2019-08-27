using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]


namespace com.brgs.orm.Azure
{
    public interface IAzureTableBuilder
    {
        Task<T> GetAsync<T>(TableQuery query, string collection);
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate);
        Task<int> PostAsync<T>(T record);
        Task<int> PostBatchAsync<T>(IEnumerable<T> records, string partition);
        void DeleteBatchAsync<T>(IEnumerable<T> records);
    }
    public class AzureTableBuilder: AzureStorageFactory,  IAzureTableBuilder
    {
        
        public AzureTableBuilder(ICloudStorageAccount acc)
        {
            _account = acc;
            _tableclient = _account.CreateCloudTableClient();
        }
        public AzureTableBuilder(ICloudStorageAccount acc, Dictionary<string, string> keys)
        {
            _account = acc;
            CollectionName = keys["CollectionName"];
            PartitionKey = keys["PartitionKey"];
            
        }

        public async Task<T> GetAsync<T>(TableQuery query, string collection)
        {
           return await InternalGetAsync<T>(query, collection);  
        }

        public override async Task<int> PostAsync<T>(T record)
        {
            return await base.PostAsync<T>(record);
    
        }
        ///<summary>each individual batch needs to be less than or equal to 100</summary>
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records, string partition)
        {
            return await base.PostBatchAsync<T>(records);
        }
    }
}