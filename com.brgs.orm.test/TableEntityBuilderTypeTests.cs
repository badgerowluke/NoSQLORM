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
    internal class DemoEntity
    {
        public bool BoolProp { get; set; }
        public double DoubleProp { get; set; }

    }
    public class TableEntityPropertyTests
    {
        [Fact]
        public void TableEntityBuilder_EncodesBoolProp()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true
            };
            var helper = new AzureFormatHelper(string.Empty);
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
            var helper = new AzureFormatHelper(string.Empty);
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
            var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("DoubleProp"));
            Assert.Equal(Convert.ToDouble(42), entity.Properties["DoubleProp"].DoubleValue);
        }
        [Fact]
        public void TableEntityBuilder_RetrievesAppropriateDoubleValue()
        {
            var demo = new DemoEntity()
            {
                DoubleProp = Convert.ToDouble(42)
            };
            var helper = new AzureFormatHelper(string.Empty);
            var entity = helper.BuildTableEntity(demo);
            Assert.Equal(Convert.ToDouble(42), entity.Properties["DoubleProp"].DoubleValue);            
        }
    }
}