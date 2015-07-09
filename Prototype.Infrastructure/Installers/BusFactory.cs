using EasyNetQ;

namespace Prototype.Infrastructure.Installers
{
    public class BusFactory
    {
        public static IBus CreateMessageBus()
        {
            // todo move these in to configs
            return RabbitHutch.CreateBus("host=192.168.137.95;virtualHost=/;username=guest;password=guest");
        }
    }
}