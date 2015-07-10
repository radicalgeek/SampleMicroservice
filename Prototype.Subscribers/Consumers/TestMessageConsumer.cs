using Prototype.Logger;
using EasyNetQ.AutoSubscribe;
using Newtonsoft.Json;

using Prototype.MessageTypes.Messages;

namespace Prototype.Subscribers.Consumers
{
    public class TestMessageConsumer : IConsume<TestMessage>
    {
        private ILogger _logger;


        public TestMessageConsumer(ILogger logger)
        {
            _logger = logger;
        }


        public void Consume(TestMessage message)
        {
            _logger.Info("Message recieved {0}", message.Message);
        }

    }
}