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
            var recivedMessages = new List<JObject>();

            dynamic message = new ExpandoObject();
            message.Message = messageData;


            logicClass.PublishSuccessMessage(message, newEntities);

            bus.Subscribe<JObject>("my_subscription_id", msg => recivedMessages.Add(msg));

            //var serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            //dynamic dynamicMessageObject = serializer.Deserialize(recivedMessages[0], typeof(object));

        }
    }
}
