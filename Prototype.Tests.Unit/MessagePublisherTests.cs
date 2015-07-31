using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using EasyNetQ;
using EasyNetQ.Topology;
using Moq;
using NUnit.Framework;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.Service.Publish;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{

    [TestFixture]
    public class MessagePublisherTests
    {
        [Test]
        public void PublishAddsMessageToBus()
        {
            var bus = new Mock<IAdvancedBus>();
            var logger = new Mock<ILogger>();
            var queue = new Mock<IQueue>();
            var exchange = new Mock<IExchange>();

            var messagePublisher = new MessagePublisher(bus.Object, logger.Object,exchange.Object,queue.Object);

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "A.B");

            bus.Verify(o => o.Publish<object>(It.IsAny<IExchange>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IMessage<object>>()), Times.Once());
        }
        
        [Test]
        public void PublishLogsMessageBeingPublished()
        {
            var bus = new Mock<IAdvancedBus>();
            var logger = new Mock<ILogger>();
            var queue = new Mock<IQueue>();
            var exchange = new Mock<IExchange>();

            var messagePublisher = new MessagePublisher(bus.Object, logger.Object, exchange.Object, queue.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(),It.IsAny<Object[]>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "A.B");

            Assert.IsTrue(invocations[1].Contains("Publishing Message: "));
        }

        [Test]
        public void PublishLogsMessageIsPublished()
        {
            var bus = new Mock<IAdvancedBus>();
            var logger = new Mock<ILogger>();
            var queue = new Mock<IQueue>();
            var exchange = new Mock<IExchange>();

            var messagePublisher = new MessagePublisher(bus.Object, logger.Object, exchange.Object, queue.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>()))
                .Callback<string, Object[]>((message, obj) => invocations.Add(message));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "A.B");

            Assert.IsTrue(invocations[1].Contains("Publish Message succeded"));
        }

        [Test]
        public void PublishFailiureLogsErrorMessage()
        {
            var bus = new Mock<IAdvancedBus>();
            var logger = new Mock<ILogger>();
            var queue = new Mock<IQueue>();
            var exchange = new Mock<IExchange>();

            var messagePublisher = new MessagePublisher(bus.Object, logger.Object, exchange.Object, queue.Object);
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));
            bus.Setup(busObj => busObj.Publish<object>(It.IsAny<IExchange>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IMessage<object>>()))
                .Throws(new EasyNetQException("Test error"));

            messagePublisher.Publish(TestMessages.GetTestNeedUserMessage(), "A.B");

            Assert.IsTrue(invocations[0].Contains("Publish Message Failed: "));
        }

        
    }
}
