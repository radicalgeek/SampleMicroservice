using EasyNetQ;
using EasyNetQ.Topology;
using Environment = Prototype.Service.Settings.Environment;

namespace Prototype.Service.Factories
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
