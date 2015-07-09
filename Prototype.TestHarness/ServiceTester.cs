using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Installers;
using Prototype.MessageTypes.Messages;
using Prototype.MessageTypes.Requests;
using Prototype.MessageTypes.Responses;
using Topshelf;
using Topshelf.Logging;

namespace CreateRequestServiceTester.Service
{
    public class ServiceTester : ServiceControl
    {
        private IWindsorContainer _container;
        private readonly LogWriter _logger = HostLogger.Get<ServiceTester>();

        public bool Start(HostControl hostControl)
        {
            _container = new WindsorContainer()
                .Install(
                    new LoggingInstaller(),
                    new BusInstaller()
                );


            _container.Register(
                Component.For<IMessagePublisher>().ImplementedBy(typeof(MessagePublisher)));

            var requestSender = _container.Resolve<IMessagePublisher>();

            var request = new TestRequest { Request = "Hello this is a request from a remote service" };

            requestSender.Request<TestRequest, TestResponse>(request, response => 
                _logger.InfoFormat("Responce received {0}", response.Response));

            var message = new TestMessage {Message = " ### Hello this is a message from a remote service ###"};

            requestSender.Publish(message);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _container.Dispose();
            _logger.Info("Stopped!");
            return true;
        }
    }
}