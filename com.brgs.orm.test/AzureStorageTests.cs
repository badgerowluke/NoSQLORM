using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;

using com.brgs.orm;

namespace com.brgs.orm.test
{
    public class AzureStorageTests
    {
        
        [Fact(Skip="because appsettings.json cannot be published")]
        public void WeDoGetAStorageFactory()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");
            var fac = new AzureStorageFactory(connectionString);
            Assert.NotNull(fac);

        }
        [Fact(Skip="because appsettings.json cannot be published")]
        public void AzureFactory_DoesReturnResults()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");
            var fac = new AzureStorageFactory(connectionString)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<List<River>>(query);
            Assert.NotEmpty(stuff);

        }

        [Fact (Skip="This fails because I don't have a good plan for implementing it correctly.")]
        public void AzureFactory_UsesTheSearchContextFromUser()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");
            var fac = new AzureStorageFactory(connectionString)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));

            var stuff = fac.Get<List<River>>(query);

            Assert.InRange(stuff.Count, 1, 4);
        }
        [Fact(Skip="because appsettings.json cannot be published")]
        public void AzureFactory_Get_DoesReturnSingleEntity()
        {
            var config = new ConfigurationBuilder()

                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");            
            var fac = new AzureStorageFactory(connectionString)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<River>(query);
            Assert.IsType<River>(stuff);
        }
        [Fact(Skip="because appsettings.json cannot be published")]
        public void AzureFactory_Get_Handles_ListKeyValuePair()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");            
            var fac = new AzureStorageFactory(connectionString)
            {
                CollectionName = "RiversUnitedStates"
            };
            var query = new TableQuery()
              .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "search"));
            var stuff = fac.Get<List<KeyValuePair<string,string>>>(query);
            Assert.IsType <List<KeyValuePair<string, string>>> (stuff);
        }
        [Fact(Skip="because appsettings.json cannot be published")]
        public void AzureFactory_Get_PullsPartitionAndRowKey()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("store");            
            var fac = new AzureStorageFactory(connectionString)
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
