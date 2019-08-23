using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;

namespace com.brgs.orm.Azure
{
    public interface ICloudStorageAccount
    {
        CloudBlobClient CreateCloudBlobClient();
        CloudTableClient CreateCloudTableClient();
        IDocumentClient CreateDocumentClient();
        CloudQueueClient CreateCloudQueueClient();
    }
    public class CloudStorageAccountBuilder: ICloudStorageAccount
    {
        private readonly CloudStorageAccount _account;
        private readonly string _url;
        private readonly string _authKey;

        public CloudStorageAccountBuilder(string connectionString)
        {
            if(connectionString.Contains("DefaultEndpointsProtocol"))
            {
                _account = CloudStorageAccount.Parse(connectionString);
            }
            if(connectionString.Contains("AccountEndpoint"))
            {
                _url = connectionString.Split(';')[0].Split('=')[1];
                _authKey = connectionString.Split(';')[1].Split(new[]{'='},2)[1];
            }

        }
        public CloudStorageAccountBuilder(string url, string key)
        {
            _url = url;
            _authKey = key;
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
        public CloudQueueClient CreateCloudQueueClient()
        {
            return _account.CreateCloudQueueClient();
        }
        public IDocumentClient CreateDocumentClient()
        {
            if(string.IsNullOrEmpty(_url)) { throw new ArgumentException("need a collection url"); }
            if(string.IsNullOrEmpty(_authKey)) { throw new ArgumentException("need an authorization key"); }
            return new DocumentClient(new Uri(_url), _authKey);
        }
    }
}