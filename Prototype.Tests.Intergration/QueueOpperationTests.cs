using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using MongoRepository;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype.Service.Consume;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Factories;
using Prototype.Service.Filters;
using Prototype.Service.Messages;
using Prototype.Service.Publish;
using Prototype.Service.Routing;
using Prototype.Service.Settings;
using Prototype.Service.Subscribe;
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
            var env = new Mock<IEnvironment>();
            var bus = BusFactory.CreateMessageBus();
            var queue = QueueFactory.CreatQueue(bus);
            var exchange = ExchangeFactory.CreatExchange(bus);
            bus.Bind(exchange, queue, "A.*");
            var publisher = new MessagePublisher(bus, logger.Object, exchange,queue);

            var dataOps = new DataOperations(logger.Object, publisher,repo.Object,env.Object);
            var filter = new MessageFilter(env.Object);
            var logicClass = new MessageRouter(env.Object, dataOps, filter);
            var messageData = TestMessages.GetTestCreateSampleEntityMessage();
            env.Setup(e => e.GetServiceName()).Returns("Fake-Service");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

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

            var consumer = bus.Consume<object>(queue, (message, info) =>  
                 Task.Factory.StartNew(() =>  
                      recivedMessages.Add(message)
                     )
                 );

            logicClass.RouteSampleMessage(messageData);
            System.Threading.Thread.Sleep(5000);

            consumer.Dispose();
            Assert.IsTrue(recivedMessages.Count >= 1);
            
        }

        [Test]
        public void ServiceConsumesMessage()
        {
            var env = new Mock<IEnvironment>();
            var logger = new Mock<ILogger>();
            var messageConsumer = new Mock<IMessageConsumer>();
            var bus = BusFactory.CreateMessageBus();
            var queue = QueueFactory.CreatQueue(bus);
            var exchange = ExchangeFactory.CreatExchange(bus);
            bus.Bind(exchange, queue, "A.*");

            var messageData = TestMessages.GetTestCreateSampleEntityMessage();
            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(messageData);
            var messageContainer = new Message<dynamic>(serializedMessage);

            var messageSubscriber = new MessageSubscriber(bus, messageConsumer.Object, logger.Object, env.Object, exchange, queue);
            messageSubscriber.Start();

            bus.Publish(exchange, "A.B", false, false, messageContainer);
            System.Threading.Thread.Sleep(10000);
            messageConsumer.Verify(c => c.Consume(It.IsAny<object>()), Times.Once());
            messageSubscriber.Stop();
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
