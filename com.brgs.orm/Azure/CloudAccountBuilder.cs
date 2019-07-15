using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace com.brgs.orm.Azure
{
    public interface ICloudStorageAccount
    {
        CloudBlobClient CreateCloudBlobClient();
        CloudTableClient CreateCloudTableClient();
        IDocumentClient CreateDocumentClient(string url, string key);
    }
    public class CloudStorageAccountBuilder: ICloudStorageAccount
    {
        private readonly CloudStorageAccount account;
        public CloudStorageAccountBuilder(string connectionString)
        {
            account = CloudStorageAccount.Parse(connectionString);
        }
        public CloudStorageAccountBuilder(CloudStorageAccount acc)
        {
            account  = acc;
        }
        public CloudBlobClient CreateCloudBlobClient()
        {
            return account.CreateCloudBlobClient();
        }
        public CloudTableClient CreateCloudTableClient()
        {
            return account.CreateCloudTableClient();
        }
        public IDocumentClient CreateDocumentClient(string url, string key)
        {
            return new DocumentClient(new Uri(url), key);
        }
    }
}