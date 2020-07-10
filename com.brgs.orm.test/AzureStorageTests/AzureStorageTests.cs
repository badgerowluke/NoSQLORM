using System.Collections.Generic;
using Xunit;
using Moq;
using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTestsShould : BaseAzureTableStorageTester
    {
        
        [Fact]
        public void Instantiate()
        {
            var fac = new AzureTableBuilder(AccountMock.Object);
            Assert.NotNull(fac);
        }
        [Fact]
        public async void ReturnResults()
        {
            var mock = GetTableQuerySegments();

            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);
            var fac = new AzureTableBuilder(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = await fac.Get<List<River>>(query);
            Assert.NotEmpty(stuff);

        }

        [Fact]
        public async void UseTheSearchContextFromUser()
        {
            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            var fac = new AzureTableBuilder(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));

            var stuff = await fac.Get<List<River>>(query);

            Assert.InRange(stuff.Count, 1, 4);
        }
        [Fact]
        public async void DoesReturnSingleEntity()
        {

            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);


            var fac = new AzureTableBuilder(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = await fac.Get<River>(query);
            Assert.IsType<River>(stuff);
        }
        [Fact]
        public async void ReturnsListKeyValuePair()
        {
            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);
            var fac = new AzureTableBuilder(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = await fac.Get<List<KeyValuePair<string,string>>>(query);
            Assert.IsType<List<KeyValuePair<string, string>>> (stuff);
        }
    }
}
