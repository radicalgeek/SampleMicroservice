using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Prototype.Infrastructure.Factory
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
