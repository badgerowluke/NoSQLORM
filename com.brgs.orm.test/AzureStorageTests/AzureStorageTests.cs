using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Reflection;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test.Azure
{
    public class AzureStorageTests : BaseAzureTableStorageTester
    {
        
        [Fact]
        public void ShouldInstantiate()
        {
            var fac = new AzureStorageFactory(AccountMock.Object);
            Assert.NotNull(fac);
        }
        [Fact]
        public void AzureFactory_DoesReturnResults()
        {
            var mock = GetTableQuerySegments();

            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<List<River>>(query);
            Assert.NotEmpty(stuff);

        }

        [Fact]
        public void AzureFactory_UsesTheSearchContextFromUser()
        {
            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));

            var stuff = fac.Get<List<River>>(query);

            Assert.InRange(stuff.Count, 1, 4);
        }
        [Fact]
        public void AzureFactory_Get_DoesReturnSingleEntity()
        {

            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);


            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<River>(query);
            Assert.IsType<River>(stuff);
        }
        [Fact]
        public void AzureFactory_Get_Handles_ListKeyValuePair()
        {
            var mock = GetTableQuerySegments();
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<List<KeyValuePair<string,string>>>(query);
            Assert.IsType <List<KeyValuePair<string, string>>> (stuff);
        }
        [Fact]
        public void DoesPullWithPartitionAndRowKey()
        {

            var mock = GetQuerySegmentsWithData<River>(new River()
            {
                Name = "SALMON R NR HYDER AK"
            });
            
            TableMock.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "USRivers"
            };
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "AK");
            var rowfilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "15008000");
            var query = new TableQuery();

            query.Where(TableQuery.CombineFilters(filter,TableOperators.And, rowfilter));
            var stuff = fac.Get<River>(query);

            Assert.Equal("SALMON R NR HYDER AK", stuff.Name);
        }
        [Fact]
        public void GetDoesHandleLambdaParameter()
        {
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                CollectionName = "USRivers"
            };

            fac.Get<River>(r => r.StateCode.Equals("WV"));
        }
    }

}
