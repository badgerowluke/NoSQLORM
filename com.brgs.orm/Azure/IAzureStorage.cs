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
        T Get<T>(TableQuery query);   
        
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate);
        Task<int> PostBatchAsync<T>(IEnumerable<T> records);     

        Task<string> Post<T>(T value, string container);
        Task<string> PostAsync<T>(T value);
        string Peek(string container);
        Task<int> GetApproximateQueueMessageCount(string container);

    }
}