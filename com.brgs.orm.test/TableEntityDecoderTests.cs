using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using com.brgs.orm.Azure.helpers;
using com.brgs.orm.Azure;

namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityDecoderShould: BaseAzureTableStorageTester
    {
        [Fact]
        public void DoesConvertITableEntityIntoDomainObject()
        {
            var entity = Builder.BuildTableEntity(ARiver);


            var testVal = (River)Builder.RecastEntity(entity, typeof(River));
            Assert.Equal(ARiver.Name, testVal.Name);
            Assert.Equal(ARiver.RiverId, testVal.RiverId);
        }
        [Fact]
        public void TableEntityDecoder_DoesDecodeDates()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.UtcNow
            };
            var entity = Builder.BuildTableEntity(demo);
            var testVal = (DemoEntity)Builder.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DateProp, testVal.DateProp);

        }        
    }
}