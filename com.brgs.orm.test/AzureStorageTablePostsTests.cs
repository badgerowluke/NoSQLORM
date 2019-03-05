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
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test
{
    public class AzureStorageTablePostTests
    {
        [Fact]
        public void WeCanTestPostingToTables()
        {
            var river = new RiverEntity()
            {
                Name = "GAULEY RIVER BELOW SUMMERSVILLE DAM, WV",
                RiverId = "03189600",
                State = "West Virginia",
                StateCode = "WV",
                Srs = "EPSG:4326",
                Latitude = "38.2151103",
                Longitude = "-80.8881536", 
                Id="Gauley|03189600",
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = Guid.NewGuid().ToString(),
                ETag = "POST",
                Timestamp = DateTime.Now

            };
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials());
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));
            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
            acc.Setup(a => a.CreateCloudTableClient()).Returns(tableClient.Object);
            var fac = new AzureStorageFactory(acc.Object)
            {
                PartitionKey = "TACOS",
                CollectionName = "Pizza"
            };


            var val = fac.Post<RiverEntity>(river);
            Assert.NotNull(val);
        }

    }
}