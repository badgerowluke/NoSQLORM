
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
using com.brgs.orm.helpers;
namespace com.brgs.orm
{
    public class AzureStorageFactory: IStorageFactory
    {
        private ICloudStorageAccount account;
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }

        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            account = acc;
        }
        public T Get<T>( string blobName)
        {
            return GetAsync<T>(CollectionName, blobName).Result;
        }
        private async Task<T> GetAsync<T>(string containerName, string blobName)
        {
            var blobClient = account.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(blobName);
            var blobStream = await blob.OpenReadAsync();
            using(StreamReader reader = new StreamReader(blobStream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public T Get<T>(TableQuery query) 
        {
            return GetAsync<T>(query).Result;
        }
        private async Task<T> GetAsync<T>(TableQuery query)
        {
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(CollectionName);
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
                        var val =  AzureFormatHelper.RecastEntity(entity, content);
                        outVal.GetType().GetMethod("Add").Invoke(outVal, new object[] { val });
                    }
                    else
                    { 
                        return (T)AzureFormatHelper.RecastEntity(entity, typeof(T));
                    }
                }

            } while (token != null);

            return outVal;
        }
        public T Post<T>(T record)
        {
            if(record is ITableEntity)
            {
                //just send the object in
                var table = PostAsync(record).Result;
            }
            else
            {
                //work it into the correct format.
                var helper = new AzureFormatHelper(PartitionKey);

                var obj = helper.BuildTableEntity(record);
                
                TableResult table = PostAsync((ITableEntity) obj).Result;
            }
            return record;
        }
        private async Task<TableResult> PostAsync<T>(T record)
        {
            try 
            {                
                var tableClient = account.CreateCloudTableClient();
                var table = tableClient.GetTableReference(CollectionName);
                bool complete = table.CreateIfNotExistsAsync().Result;
                var insert = TableOperation.InsertOrMerge((ITableEntity) record);

                var val =  await table.ExecuteAsync(insert);
                return val;

            } catch (StorageException e )
            {
                throw new Exception(e.RequestInformation.ExtendedErrorInformation.ToString());
            }
        }
    }
}