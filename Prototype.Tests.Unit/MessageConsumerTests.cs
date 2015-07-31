using System;
using System.Text;
using System.Collections.Generic;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using Prototype.Logger;
using Prototype;
using System.Web.Script.Serialization;
using Prototype.Service.Consume;
using Prototype.Service.Messages;
using Prototype.Service.Routing;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for MessageConsumerTests
    /// </summary>
    [TestFixture]
    public class MessageConsumerTests
    {

        [Test]
        public void ConsumeSendsMessageToLogicLayer()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();

            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();

            consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

            logicLayer.Verify(v => v.RouteSampleMessage(It.IsAny<object>()));
        }

        [Test]
        public void PublishLogsMessageRecived()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();
            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

           consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

           Assert.IsTrue(invocations[0].Contains("Message recieved"));
        }

        [Test]
        public void PublishLogsMessageProccessingBegun()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();
            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

            Assert.IsTrue(invocations[1].Contains("begun"));
        }

        [Test]
        public void PublishLogsMessageProccessingSucceded()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();
            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

            Assert.IsTrue(invocations[2].Contains("Succeded"));
        }

        [Test]
        public void PublishLogsMessageProccessingFailed()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();
            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<Exception>(),It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception,string, object[]>((ex, message, obj) => invocations.Add(message));
            logicLayer.Setup(c => c.RouteSampleMessage(It.IsAny<object>())).Throws(new Exception("test exception"));

            consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

            Assert.IsTrue(invocations[0].Contains("failed"));
        }

        [Test]
        public void PublishLogsExecutionTime()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<IServiceLogic>();
            var consumer = new MessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Trace(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Callback<string, object[]>((text, message) => invocations.Add(text));


            consumer.Consume(new Message<SampleMessage>( new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) }));

            Assert.IsTrue(invocations[0].Contains("proccessed in"));
        }
    }
}
