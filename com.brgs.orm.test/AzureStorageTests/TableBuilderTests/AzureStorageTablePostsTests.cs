using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;
using Moq;
using System.Threading.Tasks;

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
        public async Task PostABatchOperation(int listCount)
        {
            var list = BuildRiverEnumerable(listCount);
            var x = new TableResult();
            x.Result = list;
            x.HttpStatusCode = 200;


            TableMock.Setup(tt => tt.ExecuteBatchAsync(It.IsAny<TableBatchOperation>()))
            .ReturnsAsync(
                new List<TableResult>
                {
                    x
                });
            var val = await Fac.PostBatchAsync(list);

            


            Assert.Equal(listCount, val);
        }

    }
}