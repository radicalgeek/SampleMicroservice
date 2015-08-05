using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Prototype.Logger;
using Prototype.Service.Consume;
using Prototype.Service.Messages;
using IEnvironment = Prototype.Service.Settings.IEnvironment;

namespace Prototype.Service.Subscribe
{
    /// <summary>
    /// Automaticly subscribes to messages, by locating consumers for message types
    /// </summary>
    public class MessageSubscriber : IMessageSubscriber
    {
        private readonly IAdvancedBus _bus;
        private ILogger _logger;
        private readonly IMessageConsumer _messageConsumer;
        private IEnvironment _environment;
        private IDisposable _consumer;
        private readonly IExchange _exchange;
        private readonly IQueue _queue;

        public MessageSubscriber(IAdvancedBus bus,
            IMessageConsumer messageConsumer, 
            ILogger logger, 
            IEnvironment environment,
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
             //_bus.Consume<object>(_queue, (message, info) => _messageConsumer.Consume((IMessage<SampleMessage>)message));

             _consumer =  _bus.Consume<object>(_queue, (message, info) =>
                Task.Factory.StartNew(() =>
                    _messageConsumer.Consume(message)
                    )
                );
        }

        public void Stop()
        {
            _consumer.Dispose();
        }
    }
}
