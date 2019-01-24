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
    public class AzureFormatHelperTests
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
                Longitude = "-80.8881536"
            };

            var entity = AzureFormatHelpers.BuildTableEntity(river);
            Assert.True(entity.Properties.ContainsKey("Name"));
            Assert.Equal("River", entity.RowKey);
            Assert.NotNull(entity.PartitionKey);
        }

        [Fact]
        public void DoesConvertITableEntityIntoDomainObject()
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

            var entity = AzureFormatHelpers.BuildTableEntity(river);

            var testVal = (River)AzureFormatHelpers.RecastEntity(entity, typeof(River));
            Assert.Equal(river.Name, testVal.Name);

        }
    }
}