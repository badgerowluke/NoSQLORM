using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure
{
    public interface IAzureStorage 
    {
        string CollectionName { get; set; }
        string PartitionKey { get; set; } 
        string KeyDelimiter { get; set; }
        
        Task<T> GetAsync<T>(TableQuery query);   
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate, string collection = null);
        Task<IEnumerable<T>> GetFromStorageTableAsync<T>(Expression<Func<T,bool>> predicate = null);

        Task<string> DeleteStorageTableRecordAsync<T>(T value);
        Task<T> GetQueueMessageAsync<T>(string fileName);

        Task<string> PostAsync<T>(T value);
        Task<string> PostBlobAsync<T>(T value);
        Task<string> PostQueueMessageAsync<T>(T value, string container);
        Task<string> PostStorageTableAsync<T>(T value);
        Task<int> PostBatchAsync<T>(IEnumerable<T> records);     
        string Peek(string container);
        Task<int> GetApproximateQueueMessageCount(string container);

        

    }
}