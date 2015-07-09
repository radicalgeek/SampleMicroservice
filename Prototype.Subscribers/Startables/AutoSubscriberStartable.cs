using System.Reflection;
using Castle.Core;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Prototype.Subscribers.Dispatcher;
using Prototype.MessageTypes.Requests;
using Prototype.MessageTypes.Responses;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;

namespace Prototype.Subscribers.Startables
{
    public class AutoSubscriberStartable : IStartable
    {
        private readonly IWindsorContainer _container;
        private readonly IBus _bus;

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        public AutoSubscriberStartable(IWindsorContainer container, IBus bus)
        {

            _container = container;
            _bus = bus;

        }

        public void Start()
        {
            //todo move strings to configs
            _container.Register(Classes.FromThisAssembly().BasedOn(typeof(IConsume<>)).WithServiceSelf());


            var autoSubscriber = new AutoSubscriber(_bus, "My_subscription_id_prefix")
            {
                AutoSubscriberMessageDispatcher = new WindsorMessageDispatcher(_container, _logger)
            };

            autoSubscriber.Subscribe(Assembly.GetExecutingAssembly());
        }

        public void Stop()
        {

        }
    }
}
