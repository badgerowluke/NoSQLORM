using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;
using Moq;
using System.Threading.Tasks;
namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTableDeleteShould : BaseAzureTableStorageTester
    {
        [Fact]
        public void DeleteFromAnAzureStorageTable_AsDynamicEntity()
        {
            var val = Fac.DeleteStorageTableRecordAsync<RiverEntity>(Entity);
            Assert.NotNull(val);
        }

        [Fact]
        public void DeleteFromAnAzureStorageTable_NoPartitionKey()
        {
            Entity.PartitionKey = string.Empty;
            var val = Fac.DeleteStorageTableRecordAsync<RiverEntity>(Entity);
            Assert.NotNull(val);

        }

        [Fact]
        public void ConvertAndDeleteAPOCO()
        {
            var val = Fac.DeleteStorageTableRecordAsync<River>(ARiver);
            Assert.NotNull(val);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(101)]
        [InlineData(250)]
        public async Task DeleteBatchOperationWithDynamicEntities(int listCount)
        {
            var list = BuildRiverTableEntitiesForDelete(listCount);
            var x = new TableResult();
            x.Result = list;
            x.HttpStatusCode = 200;

            TableMock.Setup(tt => tt.ExecuteBatchAsync(It.IsAny<TableBatchOperation>()))
            .ReturnsAsync(
                new List<TableResult>
                {
                    x
                });
            var val = await Fac.DeleteBatchAsync(list);
            Assert.Equal(listCount, val);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(101)]
        [InlineData(250)]
        public async Task DeleteBatchOperationWithDynamicEntities_NoPartitionKey(int listCount)
        {
            var list = BuildRiverTableEntitiesForDelete(listCount, "TACOS", false);
            var x = new TableResult();
            x.Result = list;
            x.HttpStatusCode = 200;

            TableMock.Setup(tt => tt.ExecuteBatchAsync(It.IsAny<TableBatchOperation>()))
            .ReturnsAsync(
                new List<TableResult>
                {
                    x
                });
            var val = await Fac.DeleteBatchAsync(list);
            Assert.Equal(listCount, val);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(101)]
        [InlineData(250)]
        public async Task DeleteBatchOperation(int listCount)
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
            var val = await Fac.DeleteBatchAsync(list);
            Assert.Equal(listCount, val);
        }
        
    }
}