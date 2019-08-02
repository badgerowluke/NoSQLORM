using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace com.brgs.orm.test.Azure.Blobs
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
            var riverList = new List<River>(){
                new River()
            };
            var listString = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(riverList));

            using (var val = new MemoryStream(listString))
            {
                blob.Setup(b =>b.OpenReadAsync()).ReturnsAsync(val);
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
}