using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm
{
    public interface ICloudStorageAccount
    {
        CloudBlobClient GetCloudBlobClient();
        CloudTableClient CreateCloudTableClient();
    }
    public class CloudStorageAccountBuilder: ICloudStorageAccount
    {
        private readonly CloudStorageAccount account;
        public CloudStorageAccountBuilder(string connectionString)
        {
            account = CloudStorageAccount.Parse(connectionString);
        }
        public CloudBlobClient GetCloudBlobClient()
        {
            return account.CreateCloudBlobClient();
        }
        public CloudTableClient CreateCloudTableClient()
        {
            return account.CreateCloudTableClient();
        }
    }
}