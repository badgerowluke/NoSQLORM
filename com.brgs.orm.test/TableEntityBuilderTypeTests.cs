using System;
using Xunit;


namespace com.brgs.orm.test.Azure.Tables
{

    public class TableEntityBuilderPropertyShould : BaseAzureTableStorageTester
    {
        [Fact]
        public void EncodesBoolProp()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true
            };

            var entity = Builder.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("BoolProp"));
        }
        [Fact]
        public void RetrievesAppropriateBooleanValue()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true
            };
  
            var entity = Builder.BuildTableEntity(demo);
            Assert.True(entity.Properties["BoolProp"].BooleanValue);            
        }
        [Fact]
        public void EncodesDoubleProp()
        {
            var demo = new DemoEntity()
            {
                BoolProp = true,
                DoubleProp = Convert.ToDouble(42)
            };
            var entity = Builder.BuildTableEntity(demo);

            Assert.True(entity.Properties.ContainsKey("DoubleProp"));
        }
        [Fact]
        public void RetrievesAppropriateDoubleValue()
        {
            var demo = new DemoEntity()
            {
                DoubleProp = Convert.ToDouble(42)
            };
            var entity = Builder.BuildTableEntity(demo);
            var testVal = (DemoEntity)Builder.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DoubleProp, testVal.DoubleProp);            
        }
        [Fact]
        public void EncodesIntProp()
        {
            var demo = new DemoEntity()
            {
                IntProp = 42
            };
            var entity = Builder.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("IntProp"));
        }
        [Fact]
        public void EncodesInt64Prop()
        {
            var demo = new DemoEntity()
            {
                LongProp = Convert.ToInt64(42)
            };
            var entity = Builder.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("LongProp"));            
        }
        [Fact]
        public void CastsAndEncodesDateTime()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.Now
            };
            var entity = Builder.BuildTableEntity(demo);
            Assert.True(entity.Properties.ContainsKey("DateProp"));
        }
        [Fact]
        public void AppropriateDateValue()
        {
            var demo = new DemoEntity()
            {
                DateProp = DateTime.UtcNow
            };
            var date = DateTime.Now.ToUniversalTime();
            var entity = Builder.BuildTableEntity(demo);
            var testVal = (DemoEntity)Builder.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DateProp, testVal.DateProp);
        }

    }
}