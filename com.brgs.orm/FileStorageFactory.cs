using System;
namespace com.brgs.orm
{
    public class FileStorageFactory : IStorageFactory
    {
        public FileStorageFactory()
        {
            
        }
        public T Get<T>()
        {
            throw new NotImplementedException("coming soon");
        }
        public T Post<T>()
        {
            throw new NotImplementedException("coming soon");
        }
        public T Put<T>()
        {
            throw new NotImplementedException("coming soon");
        }
    }
}