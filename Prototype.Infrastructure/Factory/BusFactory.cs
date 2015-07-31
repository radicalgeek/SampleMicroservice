using EasyNetQ;
using System.Configuration;

namespace Prototype.Infrastructure.Factory
{
    public static class BusFactory
    {
 
      
        public static IAdvancedBus CreateMessageBus()
        {
            var environment = new HostingEnvironment();
            var connectionString = environment.GetEnvironmentVariable("RabbitMQConnectionString");
            return RabbitHutch.CreateBus(connectionString).Advanced;
        }
    }
}