using Castle.Core;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Ninject;
using Topshelf;
using Topshelf.Logging;

namespace Prototype.Service
{
    public class CreateRequestWindowsService : ServiceControl
    {
        private IKernel _container;
        private readonly LogWriter _logger = HostLogger.Get<CreateRequestWindowsService>();

        public bool Start(HostControl hostControl)
        {
            _container = new StandardKernel();
            _container.Register(Component.For<IWindsorContainer>().Instance(_container));
            _container.AddFacility<StartableFacility>(f => f.DeferredTryStart());

            _container.Install(FromAssembly.Named("Prototype.Infrastructure"));
            _container.Register(Classes.FromAssemblyNamed("Prototype.Subscribers")
                                        .BasedOn<IStartable>());

            _logger.Info("Prototype .NET Micro Service Started");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _container.Dispose();
            _logger.Info("Prototype .NET Micro Service Stopped");
            return true;
        }
    }
}