using Xunit;
using System;
namespace com.brgs.orm.test.Azure.Queues
{
    public class QueueBuilderPostShould : BaseAzureQueueBuilderTester
    {
        [Fact]
        public void ReturnAValidMessageCount()
        {
            //TODO: NEED A WAY TO FAKE THE ApproximateMessageCount

            var one = Builder.Post<River>(new River(), "favorite-river-queue");
            Assert.Equal("", one);

        }
    }
}