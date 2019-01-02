using System;
using Xunit;
using Moq;
using Microsoft.WindowsAzure.Storage;

namespace com.brgs.orm.test
{
    public class AzureStorageTests
    {
        [Fact]
        public void Test1()
        {
            var mockAccount = new Mock<CloudStorageAccount>();
            Assert.Equal(1,1);
        }
    }
}
