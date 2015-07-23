using System;
using System.Reflection;
using Prototype.Logger;
using Prototype.Subscribers.Dispatcher;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Prototype.Infrastructure;

namespace Prototype.Subscribers.Startables
{
    /// <summary>
    /// Automaticly subscribes to messages, by locating consumers for message types
    /// </summary>
    public class SampleAutoSubscriber : IAutoSubscriber
    {
        private readonly IBus _bus;

        private ILogger _logger;

        private IAutoSubscriberMessageDispatcher _messageDispatcher;

        public SampleAutoSubscriber(IBus bus, IAutoSubscriberMessageDispatcher messageDispatcher, ILogger logger)
        {
            _messageDispatcher = messageDispatcher;
            _bus = bus;
            _logger = logger;
        }

        /// <summary>
        /// Locates instances of IConsumer in this assembly and subscribes to those messages
        /// </summary>
        public void Start()
        {
            //TODO: queue name strings to configs
            var autoSubscriber = new AutoSubscriber(_bus, HostingEnvironment.GetEnvironmentVariable("QueueName"))
            {
                AutoSubscriberMessageDispatcher = _messageDispatcher
            };

            autoSubscriber.Subscribe(Assembly.GetExecutingAssembly());
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
