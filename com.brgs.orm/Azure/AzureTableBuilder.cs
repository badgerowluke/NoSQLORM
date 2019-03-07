using System;
using System.Threading.Tasks;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure
{
    internal class AzureTableBuilder
    {
        private ICloudStorageAccount account { get; set; }
        private AzureFormatHelper helpers { get; set; }
        public AzureTableBuilder(ICloudStorageAccount acc)
        {
            account = acc;
            helpers = new AzureFormatHelper();
        }
        public AzureTableBuilder(ICloudStorageAccount acc, string Collection)
        {
            account = acc;
            helpers = new AzureFormatHelper();
            
        }
        public async Task<T> GetAsync<T>(TableQuery query, string collection)
        {
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(collection);
            var outVal = (T)Activator.CreateInstance(typeof(T));
            var content = outVal.GetType().GetGenericArguments().Length > 0 ? outVal.GetType().GetGenericArguments()[0] : null ;
           
            TableContinuationToken token = null;
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(query, token);
                token = results.ContinuationToken;
                foreach(var entity in results.Results)
                {
                    if (outVal.GetType().GetMethod("Add") != null && content != null)
                    {
                        var val =  helpers.RecastEntity(entity, content);
                        outVal.GetType().GetMethod("Add").Invoke(outVal, new object[] { val });
                    }
                    else
                    { 
                        return (T)helpers.RecastEntity(entity, typeof(T));
                    }
                }

            } while (token != null);

            return outVal;            
        }
    }
}