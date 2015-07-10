using EasyNetQ;
using Ninject.Modules;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Installers;
using Prototype.Logger;
using Prototype.Subscribers.Startables;

namespace Prototype.Service.Modules
{
    public class IocModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISampleService>().To<SampleService>();
            Bind<IBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();
            Bind<ILogger>().To<Logger.Logger>().InSingletonScope();
            Bind<IMessagePublisher>().To<MessagePublisher>();
            Bind<IAutoSubscriber>().To<SampleAutoSubscriber>();

        }
    }
}