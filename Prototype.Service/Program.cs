using System.Threading;
using Prototype.Service.Services;
using Topshelf;
using Topshelf.Ninject;

namespace Prototype.Service
{
    /// <summary>
    /// The Main method that starts up and configures the service host
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {

            Thread.CurrentThread.Name = "Service Main Thread";
            
             
                var exitCode = HostFactory.Run(x =>
                {
                    x.UseNinject(new Modules.DependancyModule());

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
            return (int) exitCode;
            }
        }
    }


