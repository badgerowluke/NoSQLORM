using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace com.brgs.orm.test.Azure.Queues
{
    public class QueueBuilderGetShould : BaseAzureQueueBuilderTester
    {
        [Fact]
        public void GetMessage()
        {
            var river = new River();
            var riverString = JsonConvert.SerializeObject(river);
            
            CloudQueueMock.Setup(m => m.GetMessageAsync()).Returns(Task.FromResult(new CloudQueueMessage(riverString)));

            var msg = Builder.Get<River>("favorite-river-queue");
            Assert.IsType<River>(msg);
        }
        [Fact]
        public void ReturnNullIfNoMessage()
        {
            var msg = Builder.Get<River>("favorite-river-queue");
            Assert.Null(msg);

        }
    }
}