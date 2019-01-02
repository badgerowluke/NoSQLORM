
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace com.brgs.orm
{
    public class AzureStorageFactory : IStorageFactory
    {
        private CloudStorageAccount account;
        public T Get<T>()
        {
            throw new NotImplementedException("coming soon");
        }
        private async Task<T> GetAsync<T>(TableQuery query, string tableName)
        {
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            var outVal = (T)Activator.CreateInstance(typeof(T));

            TableContinuationToken token = null;
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(query, token);
                token = results.ContinuationToken;
                foreach(var entity in results.Results)
                {
                    var stuff = outVal.GetType().GetGenericArguments()[0];

                    outVal.GetType().GetMethod("Add").Invoke(outVal, new[] { entity });
                }

            } while (token != null);
            return outVal;
        }
        public T Post<T>()
        {
            throw new NotImplementedException("coming soon");
        }
        public T Put<T>()
        {
            throw new NotImplementedException("coming soon");   
        }
        public int Delete<T>()
        {
            throw new NotImplementedException("coming soon");
        }
    }
}