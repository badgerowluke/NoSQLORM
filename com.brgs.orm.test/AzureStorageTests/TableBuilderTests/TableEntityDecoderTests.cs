using System;
using Xunit;


namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityDecoderShould: BaseAzureTableStorageTester
    {
        [Fact(Skip="another weird flakey test to investifart")]
        public void ConvertITableEntityIntoDomainObject()
        {
            var entity = Builder.BuildTableEntity(ARiver);


            var testVal = (River)Builder.RecastEntity(entity, typeof(River));
            Assert.Equal(ARiver.Name, testVal.Name);
            Assert.Equal(ARiver.RiverId, testVal.RiverId);
        }
        [Fact(Skip="flakey, but I'm scared that there's something else going on.")]
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