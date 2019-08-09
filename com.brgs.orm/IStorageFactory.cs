

namespace com.brgs.orm
{
    public interface IStorageFactory
    {

        T Get<T>(string filename); 
        T Post<T>(T record);
        int Delete<T>(T record);
        T Put<T>(T record);
    }
}
