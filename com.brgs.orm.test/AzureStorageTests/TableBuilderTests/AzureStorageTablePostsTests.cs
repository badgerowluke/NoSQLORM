using Xunit;

namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTablePostShould : BaseAzureTableStorageTester
    {

        [Fact]
        public void PostToAnAzureStorageTable()
        {
            var val = Fac.PostAsync<RiverEntity>(Entity);
            Assert.NotNull(val);
        }
        [Fact]
        public void ConvertAndPostAPOCO()
        {
            var val = Fac.PostAsync<River>(ARiver);
            Assert.NotNull(val);
        }
        [Theory]
        [InlineData(101)]
        [InlineData(250)]
        public async  void PostABatchOperation(int listCount)
        {
            var list = BuildRiverEnumerable(listCount);
            var val = await Fac.PostBatchAsync(list);
            Assert.Equal(listCount, val);
        }

    }
}