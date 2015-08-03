using EasyNetQ;
using EasyNetQ.Topology;
using Environment = Prototype.Service.Settings.Environment;

namespace Prototype.Service.Factories
{
    public static class ExchangeFactory
    {
        public static IExchange CreatExchange(IAdvancedBus bus)
        {
            var environment = new Environment();       
            var exchange = bus.ExchangeDeclare(environment.GetEnvironmentVariable("ExchangeName"), ExchangeType.Topic);
            return exchange;
        }
    }
}
