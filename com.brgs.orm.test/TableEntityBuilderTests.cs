using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using com.brgs.orm.helpers;

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
            var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(river);
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
            var helper = new AzureFormatHelper("TestEcosystem");

            Assert.NotNull(helper.BuildTableEntity(river).PartitionKey);
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

            var helper = new AzureFormatHelper("TestEcosystem");

            var entity = helper.BuildTableEntity(river);
            Assert.Equal("TestEcosystem||03189600", entity.RowKey);
         }

    }
}