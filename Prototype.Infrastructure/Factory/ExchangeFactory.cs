using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Prototype.Infrastructure
{
    public static class ExchangeFactory
    {

        public static IExchange CreatExchange(IAdvancedBus bus)
        {
            var environment = new HostingEnvironment();
            
            var exchange = bus.ExchangeDeclare(environment.GetEnvironmentVariable("ExchangeName"), ExchangeType.Topic);
            return exchange;
        }
    }
}
