using EasyNetQ;
using EasyNetQ.Topology;
using Prototype.Infrastructure.Settings;

namespace Prototype.Infrastructure.Factories
{
    public static class QueueFactory
    {
        public static IQueue CreatQueue(IAdvancedBus bus)
        {
            var environment = new Environment();
            var queue = bus.QueueDeclare(environment.GetEnvironmentVariable("QueueName"));
            return queue;
        }
    }
}
