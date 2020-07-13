using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTablePostShould : BaseAzureTableStorageTester
    {

        [Fact]
        public void PostToAnAzureStorageTable()
        {
            var val = Fac.PostStorageTableAsync<RiverEntity>(Entity);
            Assert.NotNull(val);
        }

        [Fact]
        public void PostToAnAzureStorageTable_NoPartitionKey()
        {
            Entity.PartitionKey = string.Empty;
            var val = Fac.PostStorageTableAsync<RiverEntity>(Entity);
            Assert.NotNull(val);

        }
        [Fact]
        public void ConvertAndPostAPOCO()
        {
            var val = Fac.PostStorageTableAsync<River>(ARiver);
            Assert.NotNull(val);
        }
        [Theory]
        [InlineData(42)]
        [InlineData(101)]
        [InlineData(250)]
        public async  void PostABatchOperation(int listCount)
        {
            var list = BuildRiverEnumerable(listCount);
            var val = await Fac.PostBatchAsync(list);

            var x = new TableResult();
            


            Assert.Equal(listCount, val);
        }

    }
}