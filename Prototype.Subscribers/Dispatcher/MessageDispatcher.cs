using System;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;

namespace Prototype.Subscribers.Dispatcher
{
    public class MessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILogger _logger;
        private IConsume<IBusMessage> _consumer;

        public MessageDispatcher(ILogger logger)
        {
            _logger = logger;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            
                
                try
                {
                    _consumer.Consume((IBusMessage)message);
                }
                catch (Exception e)
                {
                    _logger.Error("Comsume Failed", e);
                }

                
            
        }
    }

}