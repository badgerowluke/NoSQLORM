
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
        private CloudStorageAccount account;
        public string CollectionName { get; set; }
        public AzureStorageFactory(string connectionString)
        {
            account = CloudStorageAccount.Parse(connectionString);
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
        public T Get<T>(TableQuery query, string table) 
        {
            return GetAsync<T>(query, table).Result;
        }
        private async Task<T> GetAsync<T>(TableQuery query, string searchFilter)
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
                        var val =  AzureFormatHelpers.RecastEntity(entity, content);
                        outVal.GetType().GetMethod("Add").Invoke(outVal, new[] { val });
                    }
                    else
                    { 
                        return (T)AzureFormatHelpers.RecastEntity(entity, typeof(T));
                    }
                }

            } while (token != null);

            return outVal;
        }
        private bool QueryHelper(object val, string searchVal)
        {
            try
            {
                List<bool> propChecks = new List<bool>();
                foreach(var prop in val.GetType().GetProperties())
                {
                    var propVal = prop.GetValue(val);
                    if(propVal != null)
                    {
                        propChecks.Add(propVal.ToString().ToUpper()
                            .Contains(searchVal.ToUpper()));
                    }
                }
                return propChecks.Contains(true);

            } catch (Exception e )
            {
                throw new Exception(e.Message);
            }
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
                var recordProps = record.GetType().GetProperties();
                var obj = AzureFormatHelpers.BuildTableEntity(recordProps, record);
                
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
        public IEnumerable<T> GetMultiple<T>(string name)
        {
            return GetEnumerableAsync<T>(CollectionName, name).Result;
        }        
        private async Task<IEnumerable<T>> GetEnumerableAsync<T>(string containerName, string blobName)
        {
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(blobName);

            var riverStream = await blob.OpenReadAsync();
            using(StreamReader reader = new StreamReader(riverStream)){
                string json = reader.ReadToEnd();
                var list = JsonConvert.DeserializeObject<IEnumerable<T>>(json);


                return list;
            }            
        }


    }
}