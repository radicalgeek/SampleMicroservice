using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factory;
using Prototype.Logger;
using Prototype.Logic;
using Prototype.Logic.DataEntities;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Serializers;
using Prototype.Tests.Unit;
using System.Web.Script.Serialization;

namespace Prototype.Tests.Intergration
{
    [TestFixture]
    public class QueueOpperationTests
    {
        [Test]
        public void ServicePublishesMessages()
        {

            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var bus = BusFactory.CreateMessageBus();
            var queue = QueueFactory.CreatQueue(bus);
            var exchange = ExchangeFactory.CreatExchange(bus);
            bus.Bind(exchange, queue, "A.*");
            var publisher = new MessagePublisher(bus, logger.Object, exchange,queue);

            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher, repo.Object, env.Object);
            var messageData = TestMessages.GetTestCreateSampleEntityMessage();

            var newEntities = new List<SampleEntity>();
            var entity = new SampleEntity
            {
                UpdatedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                NewDecimalValue = 1.02M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 3,
                NewStringValue = "test"
            };
            newEntities.Add(entity);
            var recivedMessages = new List<dynamic>();
            
            //var subscription = bus.Subscribe<dynamic>("test_id", msg => recivedMessages.Add(msg));
            //bus.Consume(queue, dispatcher => dispatcher.Add<SampleMessage>((message, info) =>
            //{
            //    recivedMessages.Add(message);
            //}));

            var consumer = bus.Consume<dynamic>(queue, (message, info) =>  
                 Task.Factory.StartNew(() =>  
                     recivedMessages.Add(message)
                     )
                 );

            System.Threading.Thread.Sleep(5000);

            logicClass.PublishSuccessMessage(messageData, newEntities, "A.B");
            System.Threading.Thread.Sleep(5000);

            consumer.Dispose();
            Assert.IsTrue(recivedMessages.Count >= 1);
            
        }

        public class TestMessage
        {
            Guid SampleUuid { get; set; }
            string Source { get; set; }
            DateTime PublishTime { get; set; }
            string ModifiedBy { get; set; }
            DateTime ModifiedTime { get; set; }
            string Method { get; set; }
            Dictionary<string,string> Needs { get; set; }
            Dictionary<string,string> Solutions { get; set; }
        }
    }
}
