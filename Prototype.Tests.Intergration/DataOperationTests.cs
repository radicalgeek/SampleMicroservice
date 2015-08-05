using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoRepository;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Publish;
using Prototype.Service.Settings;
using Prototype.Tests.Unit;

namespace Prototype.Tests.Intergration
{
    [TestFixture]
    public class DataOperationTests
    {
        [Test]
        public void ServiceCreatesDataEntity()
        {
            var env = new Mock<IEnvironment>();
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IMessagePublisher>();
            var repo = new MongoRepository<SampleEntity, string>();

            repo.DeleteAll();
            var dataOperations = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var message = TestMessages.GetTestCreateSampleEntityMessage();

            dataOperations.CreateSampleEntities(message);
            Assert.IsTrue(repo.Count() == 1);
        }

        [Test]
        public void ServiceReadsDataEntityById()
        {
            var env = new Mock<IEnvironment>();
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IMessagePublisher>();
            var repo = new MongoRepository<SampleEntity, string>();
            repo.DeleteAll();
            var publishedMessages = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<object, string>((msg, str) => publishedMessages.Add(msg));
            var id = Guid.NewGuid();
            var entity = new SampleEntity()
            {
                Id = id.ToString(),
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                NewDecimalValue = 123.12M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 123,
                NewStringValue = "SampleTestEntity",
                UpdatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime()
            };
            repo.Add(entity);
            var message = TestMessages.GetTestReadSampleEntityMessage();
            dynamic needs = new List<dynamic>();

            dynamic need = new ExpandoObject();

            need.SampleUuid = id.ToString();
            needs.Add(need);
            message.Needs = needs;

            var dataOperations = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            dataOperations.GetSampleEntities(message);

            var result = JObject.Parse(publishedMessages[0]);

            Assert.IsTrue(result.Solutions[0].SampleUuid == id);

        }

        [Test]
        public void ServiceUpdatesDataEntityById()
        {
            var env = new Mock<IEnvironment>();
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IMessagePublisher>();
            var repo = new MongoRepository<SampleEntity, string>();
            repo.DeleteAll();
            var publishedMessages = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<object, string>((msg, str) => publishedMessages.Add(msg));
            var id = Guid.NewGuid();
            var entity = new SampleEntity()
            {
                Id = id.ToString(),
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                NewDecimalValue = 123.12M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 123,
                NewStringValue = "SampleTestEntity",
                UpdatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime()
            };
            repo.Add(entity);
            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var needs = new List<dynamic>();
            dynamic need = new ExpandoObject();

            need.SampleUuid = id.ToString();
            need.NewGuidValue = Guid.NewGuid();
            need.NewStringValue = "Test";
            need.NewIntValue = 123;
            need.NewDecimalValue = 134.45M;
            needs.Add(need);
            message.Needs = needs;


            var dataOperations = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            dataOperations.UpdateSampleEntities(message);

            var result = JObject.Parse(publishedMessages[0]);

            Assert.IsTrue(result.Solutions[0].NewStringValue == "Test");
        }

        [Test]
        public void ServiceDeletesDataEntityById()
        {
            var env = new Mock<IEnvironment>();
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IMessagePublisher>();
            var repo = new MongoRepository<SampleEntity, string>();
            repo.DeleteAll();
 

            var id = Guid.NewGuid();
            var entity = new SampleEntity()
            {
                Id = id.ToString(),
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                NewDecimalValue = 123.12M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 123,
                NewStringValue = "SampleTestEntity",
                UpdatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime()
            };
            repo.Add(entity);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var needs = new List<dynamic>();
            dynamic need1 = new ExpandoObject();
            need1.SampleUuid = id;
            needs.Add(need1);
            message.Needs = needs;

            var dataOperations = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            dataOperations.DeleteSampleEntities(message);



            Assert.IsFalse(repo.Any());
        }
    }
}
