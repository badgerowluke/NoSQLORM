
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
using com.brgs.orm.Azure.helpers;

namespace com.brgs.orm.Azure
{
    public class AzureStorageFactory: AzureFormatHelper, IStorageFactory
    {
        private ICloudStorageAccount account;
        public string CollectionName { get; set; }

        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            account = acc;

        }
        public T Get<T>( string blobName)
        {
            return new AzureBlobBuilder(account)
                                .GetAsync<T>(CollectionName, blobName).Result;
        }

        public T Get<T>(TableQuery query) 
        {
            return new AzureTableBuilder(account)
                                .GetAsync<T>(query, CollectionName).Result;
        }
        public T Post<T>(T record)
        {
            var tableRunner = new AzureTableBuilder(account, CollectionName);
            if(record is ITableEntity)
            {
                //just send the object in
                var table = tableRunner.PostAsync(record).Result;
            }
            else
            {
                //work it into the correct format.
                PartitionKey = PartitionKey;

                var obj = BuildTableEntity(record);
                
                TableResult table = tableRunner.PostAsync((ITableEntity) obj).Result;
            }
            return record;
        }
    }
}