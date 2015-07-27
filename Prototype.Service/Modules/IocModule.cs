using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using MongoRepository;
using Ninject.Modules;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factory;
using Prototype.Logger;
using Prototype.Logic.DataEntities;
using Prototype.Subscribers.Dispatcher;
using Prototype.Subscribers.Startables;

namespace Prototype.Service.Modules
{
    /// <summary>
    /// Niject Module to load and manage dependancies across the lifetime of the service
    /// </summary>
    public class IocModule : NinjectModule
    {
        /// <summary>
        /// Bind Interfaces to implimentations for dependancy injection
        /// </summary>
        public override void Load()
        {
            Bind<ISampleService>().To<SampleService>();
            Bind<IBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();
            Bind<ILogger>().To<Logger.Logger>().InSingletonScope();
            Bind<IMessagePublisher>().To<MessagePublisher>();
            Bind<IAutoSubscriber>().To<SampleAutoSubscriber>();
            Bind<IAutoSubscriberMessageDispatcher>().To<MessageDispatcher>();
            Bind(typeof(IRepository<>)).To(typeof(MongoRepository<>));

        }
    }
}