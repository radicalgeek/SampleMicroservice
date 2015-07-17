using System;
using EasyNetQ;
using Prototype.Logger;


namespace Prototype.Infrastructure
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;

        public MessagePublisher(IBus bus, ILogger logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public void Publish(dynamic message)
        {
            try
            {
                _logger.Info("Publishing Message: {0}", message);
                _bus.Publish(message);
                _logger.Info("Publish Message succeded");
            }
            catch (EasyNetQException ex)
            {
                _logger.Error("Publish Message Failed: ", ex);
            }
        }
    }
}
