using EasyNetQ;
using System.Configuration;

namespace Prototype.Infrastructure.Factory
{
    public static class BusFactory
    {
        public static IBus CreateMessageBus()
        {
            HostingEnvironment.GetEnvironmentVariable("RabbitMQConnectionString");
            var connectionString = HostingEnvironment.GetEnvironmentVariable("RabbitMQConnectionString");
            return RabbitHutch.CreateBus(connectionString);
        }
    }
}