using System;
using com.brgs.orm.Azure.helpers;
using Xunit;


namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityDecoderShould: BaseAzureTableStorageTester
    {
        [Fact]
        public void GiveUsABuilder()
        {
            var builder = new TableEntityBuilder();
            Assert.NotNull(builder);
        }
        [Fact(Skip="another weird flakey test to investifart")]
        public void ConvertITableEntityIntoDomainObject()
        {
            var entity = Fac.BuildTableEntity(ARiver);


            var testVal = (River)Fac.RecastEntity(entity, typeof(River));
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
            var entity = Fac.BuildTableEntity(demo);
            var testVal = (DemoEntity)Fac.RecastEntity(entity, typeof(DemoEntity));
            Assert.Equal(demo.DateProp, testVal.DateProp);

        }        
    }
}