using System.Threading;
using Topshelf;

namespace CreateRequestServiceTester.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Sim State Service Main Thread";
            HostFactory.Run(x =>
            {
                x.Service<ServiceTester>();
                x.RunAsLocalSystem();
                x.SetDescription("Create Request Windows Service");
                x.SetDisplayName(typeof(ServiceTester).Namespace);
                x.SetServiceName(typeof(ServiceTester).Namespace);
                x.UseNLog();
            });
        }
    }
}
