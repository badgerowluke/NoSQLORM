

using System.Threading.Tasks;

namespace com.brgs.orm
{
    public interface IStorageFactory
    {

        Task<T> GetAsync<T>(string filename); 
        Task<int> PostAsync<T>(T record);
        Task DeleteAsync<T>(T record);
        T Put<T>(T record);
    }
}
