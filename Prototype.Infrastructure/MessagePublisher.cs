using System;
using EasyNetQ;
using Prototype.Logger;


namespace Prototype.Infrastructure
{
    /// <summary>
    /// Class to publish messages to the bus
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;

        public MessagePublisher(IBus bus, ILogger logger)
        {
            _bus = bus;
            _logger = logger;
        }

        /// <summary>
        /// Publishs a messaage to the bus, with no delivery verification, response or callback
        /// </summary>
        /// <param name="message">The message to be published</param>
        public void Publish(dynamic message, string topic)
        {
            try
            {
                _logger.Info("Publishing Message: {0}", message);
                if (!string.IsNullOrEmpty(topic))
                {
                    _bus.Publish(message,topic);
                }
                else
                {
                    _bus.Publish(message);
                }
                _logger.Info("Publish Message succeded");
            }
            catch (EasyNetQException ex)
            {
                _logger.Error("Publish Message Failed: ", ex);
            }
        }
    }
}
