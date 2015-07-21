using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;

namespace Prototype.Subscribers.Dispatcher
{
    /// <summary>
    /// Dispatches incoming messages to the correct IConsumer for processing based on the message type
    /// </summary>
    public class MessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILogger _logger;
        private IConsume<IBusMessage> _consumer;

        public MessageDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This is a hack constructer to allow the passing in of a consumer for
        /// testing.
        /// </summary>
        /// <param name="logger">instance of ILogger</param>
        /// <param name="consumer">instance of IConsume</param>
        public MessageDispatcher(ILogger logger, IConsume<IBusMessage> consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        /// <summary>
        /// Dispatch message to consumer
        /// </summary>
        /// <typeparam name="TMessage">the type of message (should be SampleMessge)</typeparam>
        /// <typeparam name="TConsumer">the type of consumer</typeparam>
        /// <param name="message">the actual message object to be dispatched</param>
        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            try
            {
                _consumer.Consume((IBusMessage)message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Comsume Failed for message {0}", message);
            }     
        }

        public Task DispatchAsync<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            throw new NotImplementedException();
        }
    }

}