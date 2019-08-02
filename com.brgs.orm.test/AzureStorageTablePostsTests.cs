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
            var val = Fac.Post<RiverEntity>(Entity);
            Assert.NotNull(val);
        }
        [Fact]
        public void ConvertAndPostAPOCO()
        {
            var val = Fac.Post<River>(ARiver);
            Assert.NotNull(val);
        }
        [Fact]
        public void PostABatchOperation()
        {
            var list = BuildRiverEnumerable(50);
            var val = Fac.PostBatchAsync(list);
        }
        [Fact]
        public void PostALargishBatch()
        {
            var list = BuildRiverEnumerable(250);
            var val = Fac.PostBatchAsync(list);

        }
    }
}