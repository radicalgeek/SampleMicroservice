using EasyNetQ;
using EasyNetQ.Topology;
using MongoRepository;
using Ninject;
using Ninject.Modules;
using Prototype.Logger;
using Prototype.Service.Consume;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Factories;
using Prototype.Service.Filters;
using Prototype.Service.Publish;
using Prototype.Service.Routing;
using Prototype.Service.Services;
using Prototype.Service.Subscribe;
using Environment = Prototype.Service.Settings.Environment;
using IEnvironment = Prototype.Service.Settings.IEnvironment;

namespace Prototype.Service.Modules
{
    /// <summary>
    /// Niject Module to load and manage dependancies across the lifetime of the service
    /// </summary>
    public class DependancyModule : NinjectModule
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
            Bind<IMessageSubscriber>().To<MessageSubscriber>();
            Bind(typeof(IRepository<SampleEntity,string>)).To(typeof(MongoRepository<SampleEntity,string>));
            Bind<IEnvironment>().To<Environment>();
            Bind<IMessageRouter>().To<MessageRouter>();
            Bind<IDataOperations>().To<DataOperations>();
            Bind<IMessageFilter>().To<MessageFilter>();
        }
    }
}