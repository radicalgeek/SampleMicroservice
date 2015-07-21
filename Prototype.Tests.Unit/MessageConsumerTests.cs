using System;
using System.Text;
using System.Collections.Generic;
using EasyNetQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prototype.Logger;
using Prototype.Logic;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Consumers;
using System.Web.Script.Serialization;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for MessageConsumerTests
    /// </summary>
    [TestClass]
    public class MessageConsumerTests
    {

        [TestMethod]
        public void ConsumeSendsMessageToLogicLayer()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();

            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();

            consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

            logicLayer.Verify(v => v.RouteSampleMessage(It.IsAny<object>()));
        }

        [TestMethod]
        public void PublishLogsMessageRecived()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();
            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

           consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

           Assert.IsTrue(invocations[0].Contains("Message recieved"));
        }

        [TestMethod]
        public void PublishLogsMessageProccessingBegun()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();
            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

            Assert.IsTrue(invocations[1].Contains("begun"));
        }

        [TestMethod]
        public void PublishLogsMessageProccessingSucceded()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();
            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, obj) => invocations.Add(message));

            consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

            Assert.IsTrue(invocations[2].Contains("Succeded"));
        }

        [TestMethod]
        public void PublishLogsMessageProccessingFailed()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();
            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Error(It.IsAny<Exception>(),It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception,string, object[]>((ex, message, obj) => invocations.Add(message));
            logicLayer.Setup(c => c.RouteSampleMessage(It.IsAny<object>())).Throws(new Exception("test exception"));

            consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

            Assert.IsTrue(invocations[0].Contains("failed"));
        }

        [TestMethod]
        public void PublishLogsExecutionTime()
        {
            var logger = new Mock<ILogger>();
            var logicLayer = new Mock<ISampleLogic>();
            var consumer = new SampleMessageConsumer(logger.Object, logicLayer.Object);
            var serializer = new JavaScriptSerializer();
            var invocations = new List<string>();
            logger.Setup(loggerObj => loggerObj.Trace(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Callback<string, object[]>((text, message) => invocations.Add(text));


            consumer.Consume(new SampleMessage { Message = serializer.Serialize(TestMessages.GetTestNeedUserMessage()) });

            Assert.IsTrue(invocations[0].Contains("proccessed in"));
        }
    }
}
