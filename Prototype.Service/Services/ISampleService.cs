using Topshelf;

namespace Prototype.Service.Services
{
    public interface ISampleService
    {
        bool Start(HostControl hostControl);
        bool Stop(HostControl hostControl);
    }
}