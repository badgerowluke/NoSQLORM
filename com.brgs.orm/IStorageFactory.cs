

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace com.brgs.orm
{
    public interface IStorageFactory
    {

        Task<T> GetAsync<T>(string filename); 
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate);
        Task<int> PostAsync<T>(T record);
        Task DeleteAsync<T>(T record);
        T Put<T>(T record);
    }
}
