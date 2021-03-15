using System;
using System.Threading.Tasks;
using Xunit;

namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTableGetTests : BaseAzureTableStorageTester
    {
        [Fact]
        public async Task ShouldQueryAgainstEntirePartitionProvided()
        {
            var result = await Fac.GetFromStorageTableAsync<River>();
            Assert.Null(result);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenPartitionIsNullOrEmpty()
        {
            Fac.PartitionKey = string.Empty;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await Fac.GetFromStorageTableAsync<River>());
        }

        [Fact]
        public async Task ShouldQueryWithALambda()
        {
            var result = await Fac.GetFromStorageTableAsync<River>(r => r.StateCode == "WV");
            Assert.Null(result);
        }

        [Fact]
        public async Task ShouldQueryWithGetAsyncT()
        {
            var result = await Fac.GetAsync<River>(r => r.StateCode == "WV");
            Assert.Null(result);
        }

    }
}