using System;
using Castle.Core.Logging;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;

namespace Prototype.Subscribers.Dispatcher
{
    public class WindsorMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IWindsorContainer _container;

        private readonly ILogger _logger;

        public WindsorMessageDispatcher(IWindsorContainer container, ILogger logger)
        {
            this._container = container;
            _logger = logger;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            using (_container.BeginScope()) //this ensures data is persisted once the consume is complete
            {
                var consumer = (IConsume<TMessage>)_container.Resolve<TConsumer>();
                try
                {
                    consumer.Consume(message);
                }
                catch (Exception e)
                {
                    _logger.Error("Comsume Failed", e);
                }

                finally
                {
                    _container.Release(consumer);
                }
            }
        }
    }

}