using System;
using CreateRequestServiceTester.Service;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Topshelf;

namespace Prototype.TestHarness
{
    public class ServiceTester : ServiceControl, IServiceTester
    {

        private readonly ILogger _logger;
        private IMessagePublisher _messagePublisher;

        public ServiceTester(IMessagePublisher messagePublisher, ILogger logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;

        }

        public bool Start(HostControl hostControl)
        {

            var message = new SampleMessage {Message = " ### Hello this is a message from a remote service ###"};

            try
            {
                _messagePublisher.Publish(message);

            }
            catch (Exception)
            {
                
                throw;
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {

            _logger.Info("Stopped!");
            return true;
        }
    }
}