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
        [Ignore]
        public void ServicePublishesMessages()
        {
            
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var bus = BusFactory.CreateMessageBus(); 
            var publisher = new MessagePublisher(bus,logger.Object);
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
            var recivedMessages = new List<SampleMessage>();

            dynamic message = new ExpandoObject();
            message.Message = messageData;

            var queue = bus.Subscribe<SampleMessage>("test_id", msg => recivedMessages.Add(msg));
            

            bus.Publish(new SampleMessage
                        {
                            Message = "this is a test"
                        });

            //logicClass.PublishSuccessMessage(new SampleMessage
            //            {
            //                Message = "this is a test"
            //            }, 
            //            newEntities,
            //            "");

            System.Threading.Thread.Sleep(10000);

            //var serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            //dynamic dynamicMessageObject = serializer.Deserialize(recivedMessages[0], typeof(object));

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
