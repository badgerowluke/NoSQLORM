using Xunit;
using Moq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace com.brgs.orm.test.Azure.Queues
{
    public class QueueBulderPeekShould : BaseAzureQueueBuilderTester
    {
        [Fact]
        public void ReturnAValidObject()
        {
            var river  = new River();
            var riverString = JsonConvert.SerializeObject(river);
            CloudQueueMock.Setup(m => m.PeekMessageAsync()).Returns(Task.FromResult(new CloudQueueMessage(riverString)));
            var msg = Builder.Peek("favorite-river-queue");
            Assert.IsType<string>(msg);
        }
        [Fact]
        public void ReturnsEmptyWithNoQueue()
        {
            var msg = Builder.Peek("favorite-river-queue");
            Assert.Equal(string.Empty, msg);            
        }
    }
}