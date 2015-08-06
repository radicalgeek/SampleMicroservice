using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;
using Moq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Filters;
using Prototype.Service.Publish;
using Prototype.Service.Routing;
using Prototype.Service.Settings;

namespace Prototype.Tests.Unit
{
    [TestFixture]
    class MessageFilterTests
    {

        [Test]
        public void ShouldProceesReturnsTrueWhenMessageLastModifiedByAnotherService()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");

            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldProceesReturnsFalseWhenMessageLastModifiedByThisService()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");

            message.ModifiedBy = "SampleService";

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldProceesReturnsTrueWhenRequiredVersionNotSet()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldProceesReturnsTrueWhenRequiredVersionLowerThanServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);



            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldProceesReturnsFalseWhenRequiredVersionHigherThanServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 3;

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldProceesReturnsTrueWhenRequiredVersionSameAsServiceVersion()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }


        [Test]
        public void ShouldProceesReturnsFalseWhenNeedsIsNull()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;
            message.Needs = null;

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldProceesReturnsFalseWhenNeedsIsForDiffrentNeed()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;
            var needs = new List<dynamic>();

            dynamic need1 = new ExpandoObject();
            need1.AnotherUuid = "test";
            needs.Add(need1);
            message.Needs = needs;
            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldProceesReturnsFalseSolutionExists()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;
            dynamic solution = new ExpandoObject();
            solution.SampleUuid = "test";
            var solutions = new List<dynamic>();
            solutions.Add(solution);
            message.Solutions = solutions;

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldProceesReturnsTrueWhenNoSolutionExists()
        {
            var publisher = new Mock<IMessagePublisher>();
            var logger = new Mock<ILogger>();
            var repo = new Mock<IRepository<SampleEntity>>();
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var message = TestMessages.GetTestVersion1Message();
            env.Setup(e => e.GetServiceName()).Returns("SampleService");
            env.Setup(e => e.GetServiceVersion()).Returns(2);

            message.CompatibleServiceVersions[0].Version = 2;

            var result = filter.ShouldTryProcessingMessage(message);

            Assert.IsTrue(result);
        }
    }
}
