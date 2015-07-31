using EasyNetQ;
using EasyNetQ.Topology;

namespace Prototype.Infrastructure.Factories
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
