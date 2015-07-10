using System;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Installers;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Topshelf;
using Topshelf.Logging;

namespace CreateRequestServiceTester.Service
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

            var message = new TestMessage {Message = " ### Hello this is a message from a remote service ###"};

            _messagePublisher.Publish(message);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {

            _logger.Info("Stopped!");
            return true;
        }
    }
}