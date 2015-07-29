using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using EasyNetQ.AutoSubscribe;
using Moq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Dispatcher;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for MessageDispatcherTests
    /// </summary>
    [TestFixture]
    public class MessageDispatcherTests
    {
        [Test]
        public void MessageIsDispatchedToCunsumer()
        {
            var logger = new Mock<ILogger>();
            var consumer = new Mock<IConsume<IBusMessage>>();
            var dispatcher = new MessageDispatcher(logger.Object, consumer.Object);           
            var serializer = new JavaScriptSerializer();

            var message = new SampleMessage()
            {
                Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage())
            };

            dispatcher.Dispatch<IBusMessage, IConsume<IBusMessage>>(message);

            consumer.Verify(c => c.Consume(It.IsAny<IBusMessage>()),Times.Once());
        }


        [Test]
        public void MessageDispatchFaliureLogsError()
        {
            var logger = new Mock<ILogger>();
            var consumer = new Mock<IConsume<IBusMessage>>();
            var dispatcher = new MessageDispatcher(logger.Object, consumer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<Exception>(),It.IsAny<string>(), It.IsAny<object[]>()))
               .Callback<Exception, string, object[]>((ex, errorMessage, obj) => invocations.Add(errorMessage));
            consumer.Setup(c => c.Consume(It.IsAny<IBusMessage>())).Throws(new Exception("Test Exception"));

            var message = new SampleMessage()
            {
                Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage())
            };

            dispatcher.Dispatch<IBusMessage, IConsume<IBusMessage>>(message);

            Assert.IsTrue(invocations[0].Contains("Comsume Failed for message"));
        }
    }
}
