using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public class FileStorageFactory : IStorageFactory
    {
    
        public FileStorageFactory()
        {
            
        }
        public async Task<T> GetAsync<T>(string filename)
        {
            throw new NotImplementedException();
        }
        public Task<int> PostAsync<T>(T record)
        {
            throw new NotImplementedException();
        }

        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public Task DeleteAsync<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }        
    }
}