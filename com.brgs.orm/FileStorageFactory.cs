using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public class FileStorageFactory : IStorageFactory
    {
        public string CollectionName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FileStorageFactory()
        {
            
        }
        public IEnumerable<T> GetMultiple<T>(string filename)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string filename)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(TableQuery query, string filter)
        {
            throw new NotImplementedException();
        }

        public T Post<T>(T record)
        {
            throw new NotImplementedException();
        }
    }
}