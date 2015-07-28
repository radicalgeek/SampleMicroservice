using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using EasyNetQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository;
using Moq;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.Logic;
using Prototype.Logic.DataEntities;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for SampleBusinessLogicTests
    /// </summary>
    [TestClass]
    public class SampleBusinessLogicTests
    {

        #region Create

        [TestMethod]
        public void PostOperationCreatesSingleDataEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessage());

            Assert.IsTrue(repo.Entities.Count.Equals(1));
        }

        [TestMethod]
        public void PostOperationCreatesMultipleDataEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(repo.Entities.Count.Equals(2));
        }

        [TestMethod]
        public void PostOperationLogsStoringOfEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(),It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[0].Contains("Storing new SampleEntities from message"));
        }

        [TestMethod]
        public void PostOperationLogsSuccessfullStoreOfEntities()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[1].Contains("New SampleEntity"));
        }

        [TestMethod]
        public void PostOperationWith2SolutionsLogsSuccessfullStoreOfEntitieTwice()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[1].Contains("New SampleEntity"));
            Assert.IsTrue(invocations[2].Contains("New SampleEntity"));
            Assert.IsTrue(invocations.Count == 3);
        }

        [TestMethod]
        public void PostOperationLogsErrorIfUnableToStore()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var invocations = new List<string>();
            logger.Setup(l => l.Error(It.IsAny<Exception>(),It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception,string, object[]>((ex,message, obj) => invocations.Add(message));
            repo.Setup(r => r.Add(It.IsAny<IEnumerable<SampleEntity>>())).Throws(new Exception("Test exception"));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(invocations[0].Contains("Unable to store new SampleEntities from message "));

        }

        [TestMethod]
        public void PostOperationRaisesNewEvent()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<object>(msg => messageInvocations.Add(msg));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(messageInvocations.Count > 0);

        }

        [TestMethod]
        public void PostOperationRaisesEventWithSolutions()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<object>(msg => messageInvocations.Add(msg));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(messageInvocations[0].Solutions.Count > 0);

        }

        [TestMethod]
        public void PostOperationRaisesEventWithCorrectSampleUuid()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);
            var loggerInvocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object[]>((message, obj) => loggerInvocations.Add(message));
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<object>(msg => messageInvocations.Add(msg));
            var requestMessage = TestMessages.GetTestCreateSampleEntityMessageWithMultiple();
            logicClass.RouteSampleMessage(requestMessage);

            Assert.IsTrue(messageInvocations[0].SampleUuid == requestMessage.SampleUuid);

        }

        [TestMethod]
        public void PostOperationRaisesFailEventIfUnableToStore()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var invocations = new List<string>();
            var messageInvocations = new List<dynamic>();
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, message, obj) => invocations.Add(message));
            repo.Setup(r => r.Add(It.IsAny<IEnumerable<SampleEntity>>())).Throws(new Exception("Test exception"));
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<object>(msg => messageInvocations.Add(msg));

            logicClass.RouteSampleMessage(TestMessages.GetTestCreateSampleEntityMessageWithMultiple());

            Assert.IsTrue(messageInvocations[0].Solutions == ""); //what the hell should we check for));


        }

        #endregion

        #region Read

        [TestMethod]
        public void GetOperationRetrivesSingleEntityByPrimaryKey()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);

            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(message.Needs[0].SampleUuid.ToString() == responseList[0].Solutions[0].SampleUuid.ToString());

        }

        [TestMethod]
        public void GetOperationLogsThatItIsLookingForEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>()))
                .Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logList.Add(msg));

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logList[0].Contains("Locating SampleEntities for message:"));
        }

        [TestMethod]
        public void GetOperationLogsEntityFound()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>()))
                .Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logList.Add(msg));

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logList[1].Contains("located"));
        }

        [TestMethod]
        public void GetOperationLogsEntityNotFound()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestReadSampleEntityMessage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logList = new List<string>();
            var responseList = new List<dynamic>();

            publisher.Setup(p => p.Publish(It.IsAny<object>()))
                .Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(),It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, obj) => logList.Add(msg));
            repo.Setup(r => r.GetById(It.IsAny<string>())).Throws(new Exception("test exception"));

            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logList[0].Contains("Unable to locate SampleEntity"));
        }


        #endregion 

        #region Update

        [TestMethod]
        public void PutOperationUpdatesEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);

            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));

            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(repo.Entities[0].NewStringValue == "Updated Test");
        
        }

        [TestMethod]
        public void PutOperationLogsEntityUpdating()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[0].Contains("Updating SampleEntities from message"));

        }

        [TestMethod]
        public void PutOperationLogsEntityUpdated()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[1].Contains("SampleEntities updated for message"));

        }

        [TestMethod]
        public void PutOperationReturnsUpdatedEntity()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(responseList[0].Solutions[0].NewStringValue == "Updated Test");

        }

        [TestMethod]
        public void PutOperationLogsUpdateFailed()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestUpdateSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntityFromMessage(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception,string, object[]>((ex,msg, obj) => logResponse.Add(msg));
            repo.Setup(r => r.Update(It.IsAny<List<SampleEntity>>())).Throws(new Exception("test exception"));


            message.Needs[0].NewStringValue = "Updated Test";
            message.Needs[0].NewIntValue = 456;


            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[0].Contains("Unable to update SampleEntities for message"));

        }

        #endregion

        #region Delete

        [TestMethod]
        public void DeleteOperationRemovesEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(repo.Entities.Count == 1);

        }

        [TestMethod]
        public void DeleteOperationLogsEntityToBeRemoved()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[0].Contains("Removing entity"));

        }

        [TestMethod]
        public void DeleteOperationLogsEntityIsRemoved()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new FakeRepository();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, obj) => logResponse.Add(msg));

            repo.Entities = entitys;
            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[1].Contains("Deleted"));

        }

        [TestMethod]
        public void DeleteOperationLogsUnableToRemoveEntity()
        {

            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);

            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            var entitys = TestEntities.SetUpSampleEntities(message);
            var logResponse = new List<string>();
            var responseList = new List<dynamic>();
            publisher.Setup(p => p.Publish(It.IsAny<object>())).Callback<dynamic>(msg => responseList.Add(msg));
            logger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, msg, obj) => logResponse.Add(msg));
            repo.Setup(r => r.Delete(It.IsAny<string>())).Throws(new Exception("test exception"));

            logicClass.RouteSampleMessage(message);

            Assert.IsTrue(logResponse[0].Contains("Unable to delete entity"));

        }


        #endregion

        #region ShouldProcessTests

        [TestMethod]
        public void ShouldProceesReturnsTrueWhenMessageLastModifiedByAnotherService()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");

            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();

            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldProceesReturnsFalseWhenMessageLastModifiedByThisService()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");

            message.ModifiedBy = "SampleService";

            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldProceesReturnsTrueWhenRequiredVersionNotSet()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();

            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldProceesReturnsTrueWhenRequiredVersionLowerThanServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);
            


            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldProceesReturnsFalseWhenRequiredVersionHigherThanServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 3;

            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldProceesReturnsTrueWhenRequiredVersionSameAsServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IHostingEnvironment>();
            var logicClass = new SampleBusinessLogicClass(logger.Object, publisher.Object, repo.Object, env.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;

            var result = logicClass.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        #endregion

    }

 


}
