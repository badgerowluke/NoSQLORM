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

    public class TableEntityPropertyTests
    {
        [Fact]
        public void TableEntityBuilder_EncodesBoolProp()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("BoolProp"));
        }
        [Fact]
        public void TableEntityBuilder_RetrievesAppropriateBooleanValue()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);

            Assert.True(entity.Properties["BoolProp"].BooleanValue);            
        }
        [Fact]
        public void TableEntityBuilder_EncodesDoubleProp()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true,
                DoubleProp = Convert.ToDouble(42)
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("DoubleProp"));
        }
        [Fact]
        public void TableEntityBuilder_RetrievesAppropriateDoubleValue()
        {
            var demo = new DemoEntity()
            {
                DoubleProp = Convert.ToDouble(42)
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            var testVal = (DemoEntity)helper.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DoubleProp, testVal.DoubleProp);            
        }
        [Fact]
        public void TableEntityBuilder_EncodesIntProp()
        {
            var demo = new DemoEntity()
            {
                IntProp = 42
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("IntProp"));
        }
        [Fact]
        public void TableEntityBuilder_EncodesInt64Prop()
        {
            var demo = new DemoEntity()
            {
                LongProp = Convert.ToInt64(42)
            };
              var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("LongProp"));            
        }
        [Fact]
        public void TableEntityBuilder_CastsAndEncodesDateTime()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.Now
            };
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("DateProp"));
        }
        [Fact]
        public void TableEntityBuilder_AppropriateDateValue()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.UtcNow
            };
            var date = DateTime.Now.ToUniversalTime();
            var mockAccount = new Mock<ICloudStorageAccount>();
            var helper = new AzureTableBuilder(mockAccount.Object);
            // var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            var testVal = (DemoEntity)helper.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DateProp, testVal.DateProp);

        }

    }
}