using System.Threading;
using Topshelf;

namespace Prototype.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Service Main Thread";
            HostFactory.Run(x =>
            {
                x.Service<CreateRequestWindowsService>();
                x.RunAsLocalSystem();
                x.SetDescription("Prototype .NET Micro Service");
                x.SetDisplayName(typeof(CreateRequestWindowsService).Namespace);
                x.SetServiceName(typeof(CreateRequestWindowsService).Namespace);
                x.UseNLog();
            });
        }
    }
}
