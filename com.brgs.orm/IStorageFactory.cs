using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public interface IStorageFactory
    {
        string CollectionName { get; set; }
        string PartitionKey { get; set; }
        T Get<T>(string filename); 
        T Get<T>(TableQuery query);

        T Post<T>(T record);
    }
}
