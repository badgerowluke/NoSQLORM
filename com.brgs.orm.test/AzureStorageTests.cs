using System;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;
using com.brgs.orm;

namespace com.brgs.orm.test
{
    public class AzureStorageTests
    {
        [Fact]
        public void Test1()
        {
            var mockAccount = new Mock<CloudStorageAccount>(MockBehavior.Strict
            , new object[] {""});
            Assert.Equal(1,1);

            // var storage = new AzureStorageFactory(mockAccount.Object);

            // Assert.NotNull(storage);
        }
    }
}
