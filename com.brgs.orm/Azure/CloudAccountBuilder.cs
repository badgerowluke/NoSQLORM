using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
// using Microsoft.Azure.Cosmos.Table;

namespace com.brgs.orm.Azure
{
    public interface ICloudStorageAccount
    {
        CloudBlobClient CreateCloudBlobClient();
        CloudTableClient CreateCloudTableClient();
    }
    public class CloudStorageAccountBuilder: ICloudStorageAccount
    {
        private readonly CloudStorageAccount _account;

        public CloudStorageAccountBuilder(string connectionString)
        {
            _account = CloudStorageAccount.Parse(connectionString);

        }
        public CloudStorageAccountBuilder(CloudStorageAccount acc)
        {
            _account  = acc;
        }
        public CloudBlobClient CreateCloudBlobClient()
        {
            return _account.CreateCloudBlobClient();
        }
        public CloudTableClient CreateCloudTableClient()
        {
            return _account.CreateCloudTableClient();
        }
        public IDocumentClient CreateDocumentClient(string url, string key)
        {
            return new DocumentClient(new Uri(url), key);
        }
        public CloudQueueClient CreateCloudQueueClient()
        {
            return _account.CreateCloudQueueClient();
        }
    }
}