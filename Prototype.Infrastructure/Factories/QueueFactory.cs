using EasyNetQ;
using EasyNetQ.Topology;

namespace Prototype.Infrastructure.Factories
{
    public static class QueueFactory
    {
        public static IQueue CreatQueue(IAdvancedBus bus)
        {
            var environment = new HostingEnvironment();
            var queue = bus.QueueDeclare(environment.GetEnvironmentVariable("QueueName"));
            return queue;
        }
    }
}
