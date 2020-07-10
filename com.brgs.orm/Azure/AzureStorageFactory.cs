using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using com.brgs.orm.Azure.helpers;
using System;
using System.Linq.Expressions;
using System.IO;
using Newtonsoft.Json;

using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage.Queue;

namespace com.brgs.orm.Azure
{
    public partial class AzureStorageFactory: AzureFormatHelper, IAzureStorage
    {
        protected ICloudStorageAccount _account;
        protected CloudTableClient _tableclient { get; set; }
        protected CloudBlobClient _blobClient { get; set; }
        protected CloudQueueClient _queueClient { get; set; }
        protected IDocumentClient _client { get; set; }
        protected string DatabaseId { get;  set; }
        protected string CollectionId { get; set; }

        protected string _containerName;

        public AzureStorageFactory(ICloudStorageAccount acc)
        {
            _account  = acc;
            _queueClient = _account.CreateCloudQueueClient();
            _tableclient = _account.CreateCloudTableClient();
            _blobClient = _account.CreateCloudBlobClient();
            _client = _account.CreateDocumentClient();
        }

        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public Task DeleteAsync<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }


                  
    }
}