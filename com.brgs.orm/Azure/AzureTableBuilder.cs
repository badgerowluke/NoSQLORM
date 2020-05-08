
using System.Collections.Generic;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]


namespace com.brgs.orm.Azure
{
    public interface IAzureTableBuilder
    {
        Task<T> GetAsync<T>(TableQuery query, string collection);
        Task<int> PostBatchAsync<T>(IEnumerable<T> records, string partition);
        Task DeleteBatchAsync<T>(IEnumerable<T> records);
    }
    public class AzureTableBuilder: AzureStorageFactory,  IAzureTableBuilder
    {
        public AzureTableBuilder(ICloudStorageAccount acc)
        {
            _account = acc;
            _tableclient = _account.CreateCloudTableClient();
        }
        public async Task<T> GetAsync<T>(TableQuery query, string collection)
        {
           return await InternalGetAsync<T>(query, collection);  
        }
        public override async Task<string> PostAsync<T>(T value)
        {
            return await base.PostAsync<T>(value);
    
        }
        ///<summary>each individual batch needs to be less than or equal to 100</summary>
        public async Task<int> PostBatchAsync<T>(IEnumerable<T> records, string partition)
        {
            return await base.PostBatchAsync<T>(records);
        }

        public override async Task DeleteBatchAsync<T>(IEnumerable<T> records)
        {
            await base.DeleteBatchAsync(records);
        }





    }
}