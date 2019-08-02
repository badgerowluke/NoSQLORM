using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure
{
    public interface IAzureStorage : IStorageFactory
    {
        string CollectionName { get; set; }
        string PartitionKey { get; set; }        
        T Get<T>(TableQuery query);   
        Task<int> PostBatchAsync<T>(IEnumerable<T> records);     
    }
}