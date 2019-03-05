using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

using com.brgs.orm;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Reflection;
using System.Linq;

namespace com.brgs.orm.test
{
    public class AzureStorageBlobTests
    {
        [Fact]
        public void WeDoGetDataFromBlobStorage()
        {
            var acc = new Mock<ICloudStorageAccount>();
            var blobClient = new Mock<CloudBlobClient>(new Uri("https://ww.google.com"), new StorageCredentials());
            var blobContainer = new Mock<CloudBlobContainer>(new Uri("https://www.google.com"));

            var blob = new Mock<CloudBlob>(new Uri("https://waterfinder.blob.core.windows.net/data"));
            blob.Setup

            blobContainer.Setup(bc => bc.GetBlobReference(It.IsAny<string>())).Returns(blob.Object);
            blobClient.Setup(b => b.GetContainerReference(It.IsAny<string>())).Returns(blobContainer.Object);

            acc.Setup(c => c.CreateCloudBlobClient()).Returns(blobClient.Object);

            var fac = new AzureStorageFactory(acc.Object)
            {
                CollectionName ="PIZZA"
            };
            var stuff = fac.Get<List<River>>("TACOS");
            Assert.NotEmpty(stuff);
        }
    }
}