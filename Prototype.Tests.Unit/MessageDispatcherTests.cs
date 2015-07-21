using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using EasyNetQ.AutoSubscribe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prototype.Logger;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Dispatcher;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for MessageDispatcherTests
    /// </summary>
    [TestClass]
    public class MessageDispatcherTests
    {
        [TestMethod]
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


        [TestMethod]
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
