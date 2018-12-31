using System;

namespace com.brgs.orm
{
    public interface IStorageFactory
    {
        T Get<T>();
        T Post<T>();
        T Put<T>();
        int Delete<T>();
    }
}
