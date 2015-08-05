using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Publish;
using Prototype.Service.Routing;
using Prototype.Service.Settings;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{
    [TestFixture]
    public class DataOpperationTests
    {
        #region Create

        [Test]
        public void PostOperationCreatesSingleDataEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessage());

            Assert.IsTrue(repo.Entities.Count.Equals(1));
        }

        [Test]
        public void PostOperationCreatesMultipleDataEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(repo.Entities.Count.Equals(2));
        }

        [Test]
        public void PostOperationLogsStoringOfEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[0].Contains("Storing new SampleEntities from message"));
        }

        [Test]
        public void PostOperationLogsSuccessfullStoreOfEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[1].Contains("New SampleEntity"));
        }

        [Test]
        public void PostOperationWith2SolutionsLogsSuccessfullStoreOfEntitieTwice()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[1].Contains("New SampleEntity"));
            Assert.IsTrue(invocations[2].Contains("New SampleEntity"));
            Assert.IsTrue(invocations.Count == 3);
        }

        [Test]
        public void PostOperationLogsErrorIfUnableToStore()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo.Object, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, message, obj) => invocations.Add(message));
            repo.Setup(r => r.Add(It.IsAny<IEnumerable<SampleEntity>>())).Throws(new Exception("Test exception"));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[0].Contains("Unable to store new SampleEntities from message "));

        }

        [Test]
        public void PostOperationRaisesNewEvent()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<object, string>((msg, str) => messageInvocations.Add(msg));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(messageInvocations.Count > 0);

        }

        [Test]
        public void PostOperationRaisesEventWithSolutions()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<object, string>((msg, str) => messageInvocations.Add(msg));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            var result = JObject.Parse(messageInvocations[0]);

            Assert.IsTrue(result.Solutions.Count > 0);

        }

        [Test]
        public void PostOperationRaisesEventWithCorrectSampleUuid()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<object, string>((msg, str) => messageInvocations.Add(msg));
            var requestMessage = TestMessages.GetTestCreateSampleEntityMessageWithMultiple();
            logicClass.CreateSampleEntities(requestMessage);

            var result = JObject.Parse(messageInvocations[0]);

            Assert.IsTrue(result.SampleUuid == requestMessage.SampleUuid);

        }

        [Test]
        [Ignore]
        public void PostOperationRaisesFailEventIfUnableToStore()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo.Object, env.Object);
            var invocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, message, obj) => invocations.Add(message));
            repo.Setup(r => r.Add(It.IsAny<IEnumerable<SampleEntity>>())).Throws(new Exception("Test exception"));
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<object, string>((msg, str) => messageInvocations.Add(msg));

            logicClass.CreateSampleEntities(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[0].Contains("Test Error")); //what the hell should we check for));


        }

        #endregion

        #region Read

        [Test]
        public void GetOperationRetrivesSingleEntityByPrimaryKey()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);

            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));

            repo.Entities = entitys;
            logicClass.GetSampleEntities(message);

            var result = JObject.Parse(responseList[0]);

            Assert.IsTrue(message.Needs[0].SampleUuid.ToString() == result.Solutions[0].SampleUuid.ToString());

        }

        [Test]
        public void GetOperationLogsThatItIsLookingForEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logList.Add(msg));

            repo.Entities = entitys;
            logicClass.GetSampleEntities(message);

            Assert.IsTrue(logList[0].Contains("Locating SampleEntities for message:"));
        }

        [Test]
        public void GetOperationLogsEntityFound()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logList.Add(msg));

            repo.Entities = entitys;
            logicClass.GetSampleEntities(message);

            Assert.IsTrue(logList[1].Contains("located"));
        }

        [Test]
        public void GetOperationLogsEntityNotFound()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, obj) => logList.Add(msg));
            repo.Setup(r => r.GetById(It.IsAny<string>())).Throws(new Exception("test exception"));

            logicClass.GetSampleEntities(message);

            Assert.IsTrue(logList[0].Contains("Unable to locate SampleEntity"));
        }


        #endregion

        #region Update

        [Test]
        public void PutOperationUpdatesEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);

            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));

            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.UpdateSampleEntities(message);

            Assert.IsTrue(repo.Entities[0].NewStringValue == "Updated Test");

        }

        [Test]
        public void PutOperationLogsEntityUpdating()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.UpdateSampleEntities(message);

            Assert.IsTrue(logResponse[0].Contains("Updating SampleEntities from message"));

        }

        [Test]
        public void PutOperationLogsEntityUpdated()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.UpdateSampleEntities(message);

            Assert.IsTrue(logResponse[1].Contains("SampleEntities updated for message"));

        }

        [Test]
        public void PutOperationReturnsUpdatedEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.UpdateSampleEntities(message);

            var response = JObject.Parse(responseList[0]);

            Assert.IsTrue(response.Solutions[0].NewStringValue == "Updated Test");

        }
        
        [Test]
        public void PutOperationLogsUpdateFailed()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, obj) => logResponse.Add(msg));
            repo.Setup(r => r.Update(It.IsAny<List<SampleEntity>>())).Throws(new Exception("test exception"));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;


            logicClass.UpdateSampleEntities(message);

            Assert.IsTrue(logResponse[0].Contains("Unable to update SampleEntities for message"));

        }

        #endregion

        #region Delete

        [Test]
        public void DeleteOperationRemovesEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);

            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));


            repo.Entities = entitys;
            logicClass.DeleteSampleEntities(message);

            Assert.IsTrue(repo.Entities.Count == 1);

        }

        [Test]
        public void DeleteOperationLogsEntityToBeRemoved()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));

            repo.Entities = entitys;
            logicClass.DeleteSampleEntities(message);

            Assert.IsTrue(logResponse[0].Contains("Removing entity"));

        }

        [Test]
        public void DeleteOperationLogsEntityIsRemoved()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));

            repo.Entities = entitys;
            logicClass.DeleteSampleEntities(message);

            Assert.IsTrue(logResponse[1].Contains("Deleted"));

        }

        [Test]
        public void DeleteOperationLogsUnableToRemoveEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var logicClass = new DataOperations(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<string>())).Callback<dynamic, string>((msg, str) => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, obj) => logResponse.Add(msg));
            repo.Setup(r => r.Delete(It.IsAny<string>())).Throws(new Exception("test exception"));

            logicClass.DeleteSampleEntities(message);

            Assert.IsTrue(logResponse[0].Contains("Unable to delete entity"));

        }


        #endregion
    }
}
