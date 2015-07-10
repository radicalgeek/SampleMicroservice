using Topshelf;

namespace CreateRequestServiceTester.Service
{
    public interface IServiceTester
    {
        bool Start(HostControl hostControl);
        bool Stop(HostControl hostControl);
    }
}