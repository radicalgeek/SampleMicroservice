using Topshelf;

namespace Prototype.Service
{
    public interface ISampleService
    {
        bool Start(HostControl hostControl);
        bool Stop(HostControl hostControl);
    }
}