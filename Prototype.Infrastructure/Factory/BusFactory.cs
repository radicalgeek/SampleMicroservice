using EasyNetQ;
using System.Configuration;

namespace Prototype.Infrastructure.Factory
{
    public static class BusFactory
    {
        public static IBus CreateMessageBus()
        {
            var connectionString = ConfigurationManager.AppSettings["RabbitMQConnectionString"];
            return RabbitHutch.CreateBus(connectionString);
        }
    }
}