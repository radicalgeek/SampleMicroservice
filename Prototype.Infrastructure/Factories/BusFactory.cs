using EasyNetQ;
using Prototype.Infrastructure.Settings;

namespace Prototype.Infrastructure.Factories
{
    public static class BusFactory
    {
 
      
        public static IAdvancedBus CreateMessageBus()
        {
            var environment = new Environment();
            var connectionString = environment.GetEnvironmentVariable("RabbitMQConnectionString");
            return RabbitHutch.CreateBus(connectionString).Advanced;
        }
    }
}