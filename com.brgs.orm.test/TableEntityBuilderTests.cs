using Xunit;

namespace com.brgs.orm.test.Azure.Tables
{
    public class TableEntityBuilderShould: BaseAzureTableStorageTester
    {
        [Fact]
        public void FormatObjectIntoDynamicTableEntity()
        {

            var entity = Builder.BuildTableEntity(ARiver);
            Assert.True(entity.Properties.ContainsKey("Name"));
        }

        [Fact]
        public void HaveDynamicTableEntityWithAppropriatePartitionKey()
        {
            Builder.PartitionKey = "TestEcosystem";            
            var entity = Builder.BuildTableEntity(ARiver);

            Assert.NotNull(entity.PartitionKey);
        }
         [Fact]
         public void FormatObjectIntoDynamicTableEntity_DoesHaveAppropriateRowKey()
         {
            Builder.PartitionKey = "TestEcosystem";    
            var entity = Builder.BuildTableEntity(ARiver);

            Assert.Equal("TestEcosystem||" + ARiver.Id, entity.RowKey);
         }

    }
}