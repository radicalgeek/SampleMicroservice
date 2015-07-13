using System.Reflection;
using System.Threading;
using Prototype.Service.Modules;
using Ninject.Modules;
using Topshelf;
using Ninject;
using Topshelf.Ninject;

namespace Prototype.Service
{
    using Ninject.Modules;
    using System.Runtime.Remoting.Services;

    public class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = new TopshelfExitCode();
            Thread.CurrentThread.Name = "Service Main Thread";
            
                try
                {
                    exitCode = HostFactory.Run(x =>
                    {
                        x.UseNinject(new Prototype.Service.Modules.IocModule());

                        x.Service<SampleService>(s =>
                        {
                            s.ConstructUsingNinject();
                            s.WhenStarted((service, hostControl) => service.Start(hostControl));
                            s.WhenStopped((service, hostControl) => service.Stop(hostControl));

                        });
                        x.RunAsLocalSystem();
                        x.SetDescription("Prototype .NET Micro Service");
                        x.SetDisplayName(typeof(SampleService).Namespace);
                        x.SetServiceName(typeof(SampleService).Namespace);
                        x.UseNLog();
                    });
                }
                catch (System.Exception ex)
                {
                    
                    throw;
                }
            
            return (int) exitCode;
        }
    }
}
