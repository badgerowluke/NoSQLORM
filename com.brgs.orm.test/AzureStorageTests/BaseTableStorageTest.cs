using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;

namespace com.brgs.orm.test.Azure
{
    public abstract class BaseAzureTableStorageTester
    {
        public Mock<ICloudStorageAccount> AccountMock { get; private set; }
        public Mock<CloudTableClient> TableClientMock { get; private set; }
        public Mock<CloudTable> TableMock { get; private set; }
        public BaseAzureTableStorageTester()
        {
            AccountMock = new Mock<ICloudStorageAccount>();
            TableClientMock = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            TableMock = new Mock<CloudTable>(new Uri("https://www.google.com"));

            TableClientMock.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(TableMock.Object);

            AccountMock.Setup(c => c.CreateCloudTableClient()).Returns(TableClientMock.Object);
        }
        public virtual TableQuerySegment GetTableQuerySegments()
        {
            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == 1);
            return ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
        }

    }
}