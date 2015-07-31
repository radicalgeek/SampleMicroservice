using System;
using System.Reflection;
using EasyNetQ.Topology;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Consumers;
using Prototype.Subscribers.Dispatcher;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Prototype.Infrastructure;

namespace Prototype.Subscribers.Startables
{
    /// <summary>
    /// Automaticly subscribes to messages, by locating consumers for message types
    /// </summary>
    public class SampleSubscriber : ISubscriber
    {
        private readonly IAdvancedBus _bus;
        private ILogger _logger;
        private IMessageConsumer _messageConsumer;
        private IHostingEnvironment _environment;
        private IDisposable Consumer;
        private IExchange _exchange;
        private IQueue _queue;

        public SampleSubscriber(IAdvancedBus bus,
            IMessageConsumer messageConsumer, 
            ILogger logger, 
            IHostingEnvironment environment,
            IExchange exchange,
            IQueue queue)
        {
            _messageConsumer = messageConsumer;
            _bus = bus;
            _logger = logger;
            _environment = environment;
            _exchange = exchange;
            _queue = queue;
        }

        /// <summary>
        /// Declares a new Exchange and Queue, and registers a consumer
        /// </summary>
        public void Start()
        {     
            _bus.Bind(_exchange, _queue, "A.*");

            Consumer = _bus.Consume(_queue, dispatcher => dispatcher.Add<SampleMessage>((message, info) =>
            {
                _messageConsumer.Consume(message);
            }));
        }

        public void Stop()
        {
            Consumer.Dispose();
        }
    }
}
