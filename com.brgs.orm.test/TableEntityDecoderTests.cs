using System;
using Xunit;


namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityDecoderShould: BaseAzureTableStorageTester
    {
        [Fact]
        public void ConvertITableEntityIntoDomainObject()
        {
            var entity = Builder.BuildTableEntity(ARiver);


            var testVal = (River)Builder.RecastEntity(entity, typeof(River));
            Assert.Equal(ARiver.Name, testVal.Name);
            Assert.Equal(ARiver.RiverId, testVal.RiverId);
        }
        [Fact]
        public void DecodeDates()
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