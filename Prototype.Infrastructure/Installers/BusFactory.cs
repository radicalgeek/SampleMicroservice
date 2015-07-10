using EasyNetQ;

namespace Prototype.Infrastructure.Installers
{
    public class BusFactory
    {
        public static IBus CreateMessageBus()
        {
            // todo move these in to configs
            return RabbitHutch.CreateBus("host=192.168.59.103:5672;virtualHost=/;username=guest;password=guest");
        }
    }
}