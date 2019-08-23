using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;
using Moq;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage.Queue;

namespace com.brgs.orm.test.Azure.Queues
{
    public abstract class BaseAzureQueueBuilderTester
    {
        public Mock<ICloudStorageAccount> AccountMock { get; private set; }
        public Mock<CloudQueueClient> QueueClientMock { get; private set; }
        public Mock<CloudQueue> CloudQueueMock { get; private set; }
        internal AzureQueueBuilder Builder { get; private set; }
        public BaseAzureQueueBuilderTester()
        {
            AccountMock = new Mock<ICloudStorageAccount>();
            QueueClientMock = new Mock<CloudQueueClient>(new Uri("https://www.google.com"), new StorageCredentials());
            CloudQueueMock = new Mock<CloudQueue>(new Uri("https://www.google.com"));
            QueueClientMock.Setup(qc => qc.GetQueueReference(It.IsAny<string>())).Returns(CloudQueueMock.Object);
            AccountMock.Setup(c => c.CreateCloudQueueClient()).Returns(QueueClientMock.Object);
            Builder = new AzureQueueBuilder(AccountMock.Object);
        }
    }

}