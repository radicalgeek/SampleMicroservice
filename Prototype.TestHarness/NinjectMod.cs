using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Prototype.Infrastructure.Installers;
using Prototype.Logger;
using Topshelf;
using EasyNetQ;

namespace CreateRequestServiceTester.Service
{
    public class NinjectMod : NinjectModule
    {
        public override void Load()
        {
            Bind<IServiceTester>().To<ServiceTester>();
            Bind<IBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<IBus>().ToMethod(context => BusFactory.CreateMessageBus()).InSingletonScope();

        }
    }
}
