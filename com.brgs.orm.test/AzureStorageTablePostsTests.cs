using System;
using Xunit;
using Moq;
using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using AutoFixture;
using AutoFixture.Xunit;
using AutoFixture.AutoMoq;

namespace com.brgs.orm.test.Azure.Tables
{
    public class AzureStorageTablePostShould : BaseAzureTableStorageTester
    {

        [Fact]
        public void PostToAnAzureStorageTable()
        {
            
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                PartitionKey = "TACOS",
                CollectionName = "Pizza"
            };


            var val = fac.Post<RiverEntity>(Entity);
            Assert.NotNull(val);
        }
        [Fact]
        public void ConvertAndPostAPOCO()
        {
     
            var fac = new AzureStorageFactory(AccountMock.Object)
            {
                PartitionKey = "TACOS",
                CollectionName = "Pizza"
            };


            var val = fac.Post<River>(ARiver);
            Assert.NotNull(val);
        }

    }
}