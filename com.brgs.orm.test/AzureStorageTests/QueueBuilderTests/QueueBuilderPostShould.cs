using Xunit;
using System;
using System.Threading.Tasks;

namespace com.brgs.orm.test.Azure.Queues
{
    public class QueueBuilderPostShould : BaseAzureQueueBuilderTester
    {
        [Fact]
        public async Task ReturnAValidMessageCount()
        {
            //TODO: NEED A WAY TO FAKE THE ApproximateMessageCount

            var one = await Builder.PostQueueMessageAsync<River>(new River(), "favorite-river-queue");
            Assert.Equal("", one);

        }
    }
}