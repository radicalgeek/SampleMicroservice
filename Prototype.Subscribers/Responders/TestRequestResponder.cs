using Castle.Core.Logging;
using EasyNetQ;
using Prototype.MessageTypes.Requests;
using Prototype.MessageTypes.Responses;

namespace Prototype.Subscribers.Responders
{

    public class TestRequestResponder : IResponder
    {
        private readonly IBus _bus;

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }


        public TestRequestResponder(IBus bus)
        {
            _bus = bus;
        }
        public void Subscribe()
        {
            _bus.Respond<TestRequest, TestResponse>(Response);
        
        }

        private TestResponse Response(TestRequest request)
        {
            Logger.InfoFormat("Received request: {0}", request.Request);
           return new TestResponse {Response = " ***** Response to Request *****"};
        }
    }
}