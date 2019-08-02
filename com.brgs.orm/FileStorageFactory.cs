using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public class FileStorageFactory : IStorageFactory
    {
        public string CollectionName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string PartitionKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }        
        public FileStorageFactory()
        {
            
        }
        public T Get<T>(string filename)
        {
            throw new NotImplementedException();
        }
        public T Post<T>(T record)
        {
            throw new NotImplementedException();
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