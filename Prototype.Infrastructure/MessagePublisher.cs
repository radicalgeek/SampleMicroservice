using System;
using EasyNetQ;
using EasyNetQ.Topology;
using Prototype.Logger;


namespace Prototype.Infrastructure
{
    /// <summary>
    /// Class to publish messages to the bus
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IAdvancedBus _bus;
        private readonly ILogger _logger;
        private IExchange _exchange;
        private IQueue _queue;
        
        public MessagePublisher(IAdvancedBus bus, ILogger logger, IExchange exchange, IQueue queue)
        {
            _bus = bus;
            _logger = logger;
            _exchange = exchange;
            _queue = queue;
        }

        /// <summary>
        /// Publishs a messaage to the bus, with no delivery verification, response or callback
        /// </summary>
        /// <param name="message">The message to be published</param>
        /// <param name="topic">The topic to send the message too</param>
        public void Publish(dynamic message, string topic)
        {
            try
            {
                _logger.Info("Preparing message for queue");
                var messageContainer = new Message<dynamic>(message);

                _logger.Info("Publishing Message: {0}", message);
                _bus.Publish(_exchange, topic, false, false, messageContainer); 
                _logger.Info("Publish Message succeded");
            }
            catch (EasyNetQException ex)
            {
                _logger.Error("Publish Message Failed: ", ex);
            }
        }
    }
}
