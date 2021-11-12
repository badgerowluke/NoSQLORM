using Xunit;

namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityBuilderShould: BaseAzureTableStorageTester
    {
        [Fact]
        public void FormatObjectIntoDynamicTableEntity()
        {
            
            var entity = Fac.BuildTableEntity(ARiver);
            Assert.True(entity.Properties.ContainsKey("Name"));
        }

        [Fact]
        public void HaveDynamicTableEntityWithAppropriatePartitionKey()
        {
            Fac.PartitionKey = "TestEcosystem";            
            var entity = Fac.BuildTableEntity(ARiver);

            Assert.NotNull(entity.PartitionKey);
        }
         [Fact]
         public void FormatObjectIntoDynamicTableEntity_DoesHaveAppropriateRowKey()
         {
            Fac.PartitionKey = "TestEcosystem";    
            var entity = Fac.BuildTableEntity(ARiver);

            Assert.Equal("TestEcosystem||" + ARiver.Id, entity.RowKey);
         }

    }
}