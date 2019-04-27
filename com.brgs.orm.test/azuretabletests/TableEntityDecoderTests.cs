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
    public class TableEntityDecoderTests
    {
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
               Longitude = "-80.8881536",
               Id="Gauley|03189600"
            };

            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);

            var entity = helper.BuildTableEntity(river);

            var testVal = (River)helper.RecastEntity(entity, typeof(River));
            Assert.Equal(river.Name, testVal.Name);
            Assert.Equal(river.RiverId, testVal.RiverId);
        }
        [Fact(Skip="not sure why this barfed a build")]
        public void TableEntityDecoder_DoesDecodeDates()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.UtcNow
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            var entity = helper.BuildTableEntity(demo);
            var testVal = (DemoEntity)helper.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DateProp, testVal.DateProp);

        }        
    }
}