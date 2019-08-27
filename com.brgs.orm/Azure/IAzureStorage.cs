using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;

namespace com.brgs.orm.Azure
{
    public interface IAzureStorage : IStorageFactory
    {
        string CollectionName { get; set; }
        string PartitionKey { get; set; } 
        T Get<T>(TableQuery query);   
        
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate, string collection = null);

        Task<int> PostBatchAsync<T>(IEnumerable<T> records);     

        string Post<T>(T value, string container);
        string Peek(string container);
        Task<int> GetApproximateQueueMessageCount(string container);

    }
}