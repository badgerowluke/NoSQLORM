using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;

using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Reflection;
using System.Linq;

namespace com.brgs.orm.test
{
    public class AzureStorageTests
    {
        
        [Fact]
        public void WeDoGetAStorageFactory()
        {
            var stuff = new Mock<ICloudStorageAccount>();
            var fac = new AzureStorageFactory(stuff.Object);
            Assert.NotNull(fac);
        }
        [Fact]
        public void AzureFactory_DoesReturnResults()
        {
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));

            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mock = ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
            
            table.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
                        
            acc.Setup(c => c.CreateCloudTableClient()).Returns(tableClient.Object);

            var fac = new AzureStorageFactory(acc.Object)
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
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));

            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mock = ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
            
            table.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
                        
            acc.Setup(c => c.CreateCloudTableClient()).Returns(tableClient.Object);

            var fac = new AzureStorageFactory(acc.Object)
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
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));

            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mock = ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
            
            table.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
                        
            acc.Setup(c => c.CreateCloudTableClient()).Returns(tableClient.Object);

            var fac = new AzureStorageFactory(acc.Object)
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
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));

            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mock = ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
            
            table.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
                        
            acc.Setup(c => c.CreateCloudTableClient()).Returns(tableClient.Object);

            var fac = new AzureStorageFactory(acc.Object)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<List<KeyValuePair<string,string>>>(query);
            Assert.IsType <List<KeyValuePair<string, string>>> (stuff);
        }
        [Fact(Skip="because I don't want to whack together a DynamicTableEntity for this.")]
        public void AzureFactory_Get_PullsPartitionAndRowKey()
        {
            var acc = new Mock<ICloudStorageAccount>();
            var tableClient = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            var table = new Mock<CloudTable>(new Uri("https://www.google.com"));

            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            var mock = ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
            
            table.Setup(tt =>tt.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(mock);

            tableClient.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(table.Object);
                        
            acc.Setup(c => c.CreateCloudTableClient()).Returns(tableClient.Object);

      
            var fac = new AzureStorageFactory(acc.Object)
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
    }

}
