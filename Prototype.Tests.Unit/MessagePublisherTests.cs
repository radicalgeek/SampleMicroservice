using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{

    [TestFixture]
    public class MessagePublisherTests
    {
        [Test]
        public void PublishAddsMessageToBus()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "test");

            bus.Verify(o => o.Publish(It.IsAny<Object>(),It.IsAny<string>()),Times.Once());
        }

        [Test]
        public void PublishLogsMessageBeingPublished()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(),It.IsAny<Object[]>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "test");

            Assert.IsTrue(invocations[0].Contains("Publishing Message: "));
        }

        [Test]
        public void PublishLogsMessageIsPublished()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "test");

            Assert.IsTrue(invocations[0].Contains("Publish Message succeded"));
        }

        [Test]
        public void PublishFailiureLogsErrorMessage()
        {
            var bus = new Mock<IBus>();
            var logger = new Mock<ILogger>();
            var messagePublisher = new MessagePublisher(bus.Object, logger.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));
            bus.Setup(busObj => busObj.Publish(It.IsAny<object>(),It.IsAny<string>()))
                .Throws(new EasyNetQException("Test error"));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "test");

            Assert.IsTrue(invocations[0].Contains("Publish Message Failed: "));
        }

        
    }
}
