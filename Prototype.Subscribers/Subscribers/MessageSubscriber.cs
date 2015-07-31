using System;
using EasyNetQ;
using EasyNetQ.Topology;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Prototype.Messaging.Consumers;

namespace Prototype.Messaging.Subscribers
{
    /// <summary>
    /// Automaticly subscribes to messages, by locating consumers for message types
    /// </summary>
    public class MessageSubscriber : IMessageSubscriber
    {
        private readonly IAdvancedBus _bus;
        private ILogger _logger;
        private IMessageConsumer _messageConsumer;
        private IEnvironmentSettings _environmentSettings;
        private IDisposable Consumer;
        private IExchange _exchange;
        private IQueue _queue;

        public MessageSubscriber(IAdvancedBus bus,
            IMessageConsumer messageConsumer, 
            ILogger logger, 
            IEnvironmentSettings environmentSettings,
            IExchange exchange,
            IQueue queue)
        {
            _messageConsumer = messageConsumer;
            _bus = bus;
            _logger = logger;
            _environmentSettings = environmentSettings;
            _exchange = exchange;
            _queue = queue;
        }

        /// <summary>
        /// Declares a new Exchange and Queue, and registers a consumer
        /// </summary>
        public void Start()
        {     
            _bus.Bind(_exchange, _queue, "A.*");
            Consumer = _bus.Consume(_queue, dispatcher => dispatcher.Add<SampleMessage>((message, info) => _messageConsumer.Consume(message)));
        }

        public void Stop()
        {
            Consumer.Dispose();
        }
    }
}
