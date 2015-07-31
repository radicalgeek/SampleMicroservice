using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Topology;
using MongoRepository;
using Ninject;
using Ninject.Modules;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factories;
using Prototype.Logger;
using Prototype.Logic;
using Prototype.Logic.DataEntities;
using Prototype.Subscribers.Consumers;
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
            Bind<IAdvancedBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();
            Bind<IExchange>().ToMethod(context => ExchangeFactory.CreatExchange(context.Kernel.Get<IAdvancedBus>())).InSingletonScope();
            Bind<IQueue>().ToMethod(context => QueueFactory.CreatQueue(context.Kernel.Get<IAdvancedBus>())).InSingletonScope();
            Bind<ILogger>().To<Logger.Logger>().InSingletonScope();
            Bind<IMessagePublisher>().To<MessagePublisher>();
            Bind<IMessageConsumer>().To<MessageConsumer>();
            Bind<ISubscriber>().To<SampleSubscriber>();
            Bind(typeof(IRepository<SampleEntity,string>)).To(typeof(MongoRepository<SampleEntity,string>));
            Bind<IHostingEnvironment>().To<HostingEnvironment>();
            Bind<ISampleLogic>().To<SampleBusinessLogicClass>();
        }
    }
}