using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using EasyNetQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prototype.Infrastructure;
using Prototype.Logger;

namespace Prototype.Tests.Unit
{

    [TestClass]
    public class MessagePublisherTests
    {
        [TestMethod]
        public void PublishAddsMessageToBus()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);

            messagePublisher.Publish(GetTestNeedUserMessage());

            bus.Verify(o => o.Publish(It.IsAny<Object>()),Times.Once());
        }

        [TestMethod]
        public void PublishLogsMessageBeingPublished()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(),It.IsAny<Object[]>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(GetTestNeedUserMessage());

            Assert.IsTrue(invocations[0].Contains("Publishing Message: "));
        }

        [TestMethod]
        public void PublishLogsMessageIsPublished()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(GetTestNeedUserMessage());

            Assert.IsTrue(invocations[0].Contains("Publish Message succeded"));
        }

        [TestMethod]
        public void PublishFailiureLogsErrorMessage()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));
            bus.Setup(busObj => busObj.Publish(It.IsAny<object>()))
                .Throws(new EasyNetQException("Test error"));

            messagePublisher.Publish(GetTestNeedUserMessage());

            Assert.IsTrue(invocations[0].Contains("Publish Message Failed: "));
        }

        private dynamic GetTestNeedUserMessage()
        {

            dynamic need = new
            {
                UserUuid = Guid.NewGuid()
            };

            dynamic solutions = new
            {

            };

            dynamic message = new
            {
                Uuid = Guid.NewGuid(),
                Source = "UnitTest-Service",
                PublishTime = DateTime.Now.ToUniversalTime(),
                ModifiedBy = "UnitTest-Service",
                ModifiedTime = DateTime.Now.ToUniversalTime(),
                Method = "GET",
                Need = need,
                Solutions = solutions
            };

            return message;
        }
    }
}
