using EasyNetQ;
using Environment = Prototype.Service.Settings.Environment;

namespace Prototype.Service.Factories
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