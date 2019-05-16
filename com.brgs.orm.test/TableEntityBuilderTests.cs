using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using com.brgs.orm.Azure.helpers;
using com.brgs.orm.Azure;

namespace com.brgs.orm.test
{
    public class TableEntityBuilderTests
    {
        [Fact]
        public void DoFormatObjectIntoDynamicTableEntity()
        {
            var river = new River()
            {
                Name = "GAULEY RIVER BELOW SUMMERSVILLE DAM, WV",
                RiverId = "03189600",
                State = "West Virginia",
                StateCode = "WV",
                Srs = "EPSG:4326",
                Latitude = "38.2151103",
                Longitude = "-80.8881536", 
                Id="Gauley|03189600"

            };

            var mockAccount = new Mock<ICloudStorageAccount>();
            var fac = new AzureStorageFactory(mockAccount.Object);
            var entity = fac.BuildTableEntity(river);
            Assert.True(entity.Properties.ContainsKey("Name"));
        }

        [Fact]
        public void DoesFormatObjectIntoDynamicTableEntity_DoesHaveAppropriatePartitionKey()
        {
            var river = new River()
            {
                Name = "GAULEY RIVER BELOW SUMMERSVILLE DAM, WV",
                RiverId = "03189600",
                State = "West Virginia",
                StateCode = "WV",
                Srs = "EPSG:4326",
                Latitude = "38.2151103",
                Longitude = "-80.8881536"
            };

            var mockAccount = new Mock<ICloudStorageAccount>();
            var fac = new AzureStorageFactory(mockAccount.Object);
            fac.PartitionKey = "TestEcosystem";            

            Assert.NotNull(fac.BuildTableEntity(river).PartitionKey);
        }
         [Fact]
         public void DoesFormatObjectIntoDynamicTableEntity_DoesHaveAppropriateRowKey()
         {
            var river = new River()
            {
                Name = "GAULEY RIVER BELOW SUMMERSVILLE DAM, WV",
                RiverId = "03189600",
                State = "West Virginia",
                StateCode = "WV",
                Srs = "EPSG:4326",
                Latitude = "38.2151103",
                Longitude = "-80.8881536",
                Id = "03189600"
            };

            var mockAccount = new Mock<ICloudStorageAccount>();
            var fac = new AzureStorageFactory(mockAccount.Object);
            fac.PartitionKey = "TestEcosystem";    

            var entity = fac.BuildTableEntity(river);
            Assert.Equal("TestEcosystem||03189600", entity.RowKey);
         }

    }
}