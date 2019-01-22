using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public interface IStorageFactory
    {
        string CollectionName { get; set; }
        IEnumerable<T> GetMultiple<T>(string filename);
        T Get<T>(string filename); 
        T Get<T>(TableQuery query, string filter);
        T Post<T>(T record);
    }
}
