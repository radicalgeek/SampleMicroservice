using System.Reflection;
using Prototype.Logger;
using Prototype.Subscribers.Dispatcher;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;

namespace Prototype.Subscribers.Startables
{
    public class SampleAutoSubscriber : IAutoSubscriber
    {
        private readonly IBus _bus;

        private ILogger _logger;

        private IAutoSubscriberMessageDispatcher _messageDispatcher;

        public SampleAutoSubscriber(IBus bus, IAutoSubscriberMessageDispatcher messageDispatcher, ILogger logger)
        {
            _messageDispatcher = messageDispatcher;
            _bus = bus;
            _logger = logger;
        }

        public void Start()
        {
            //todo move strings to configs
            var autoSubscriber = new AutoSubscriber(_bus, "Sample_Queue")
            {
                AutoSubscriberMessageDispatcher = _messageDispatcher
            };

            autoSubscriber.Subscribe(Assembly.GetExecutingAssembly());
        }

        public void Stop()
        {

        }
    }
}
