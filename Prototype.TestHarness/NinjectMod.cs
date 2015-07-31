using CreateRequestServiceTester.Service;
using EasyNetQ;
using Ninject.Modules;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factories;
using Prototype.Logger;


namespace Prototype.TestHarness
{
    public class NinjectMod : NinjectModule
    {
        public override void Load()
        {
            Bind<IServiceTester>().To<ServiceTester>();
            Bind<IAdvancedBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();
            Bind<ILogger>().To<Logger.Logger>().InSingletonScope();
     
            
            Bind<IMessagePublisher>().To<MessagePublisher>();


        }
    }
}
