using System;
using Xunit;
using System.Linq;
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