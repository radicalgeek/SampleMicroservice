using System.Threading;
using CreateRequestServiceTester.Service;
using Topshelf;
using Topshelf.Ninject;

namespace Prototype.TestHarness
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Thread.CurrentThread.Name = "Service Main Thread";
            var exitCode = HostFactory.Run(x =>
            {
                x.UseNinject(new NinjectMod());

                x.Service<ServiceTester>(s =>
                {
                    s.ConstructUsingNinject();
                    s.WhenStarted((service, hostControl) => service.Start(hostControl));
                    s.WhenStopped((service, hostControl) => service.Stop(hostControl));

                });
                x.RunAsLocalSystem();
                x.SetDescription("Prototype .NET Micro Service");
                x.SetDisplayName(typeof (ServiceTester).Namespace);
                x.SetServiceName(typeof (ServiceTester).Namespace);

            });
            return (int) exitCode;
        }
    }
}
