using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EasyNetQ;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Prototype.Logger;
using NUnit;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Filters;
using Prototype.Service.Publish;
using Prototype.Service.Routing;
using Prototype.Service.Settings;
using Prototype.Tests.Helpers;

namespace Prototype.Tests.Unit
{
    /// <summary>
    /// Summary description for SampleBusinessLogicTests
    /// </summary>
    [TestFixture]
    public class MessageRoutingTests
    {
        [Test]
        public void PostOperationCallsCreateDataOperation()
        {
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var logger = new Mock<ILogger>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var logicClass = new MessageRouter(logger.Object, dataOps.Object, filter);
            var message = TestMessages.GetTestCreateSampleEntityMessage();

            logicClass.RouteSampleMessage(message);

            dataOps.Verify(d => d.CreateSampleEntities(It.IsAny<object>()), Times.Once());
        }

        [Test]
        public void GetOperationCallsGetDataOperation()
        {    
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var logger = new Mock<ILogger>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var logicClass = new MessageRouter(logger.Object, dataOps.Object, filter);
            var message = TestMessages.GetTestReadSampleEntityMessage();

            logicClass.RouteSampleMessage(message);

            dataOps.Verify(d => d.GetSampleEntities(It.IsAny<object>()),Times.Once());
        }

        [Test]
        public void PutOperationCallsUpdateDataOperation()
        {
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var logger = new Mock<ILogger>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var logicClass = new MessageRouter(logger.Object, dataOps.Object, filter);
            var message = TestMessages.GetTestUpdateSampleEntityMesssage();

            logicClass.RouteSampleMessage(message);

            dataOps.Verify(d => d.UpdateSampleEntities(It.IsAny<object>()), Times.Once());
        }

       [Test]
        public void DeleteOperationCallsDeleteDataOperation()
        {
            var env = new Mock<IEnvironment>();
            var dataOps = new Mock<IDataOperations>();
            var logger = new Mock<ILogger>();
            var filter = new MessageFilter(env.Object, logger.Object);
            var logicClass = new MessageRouter(logger.Object, dataOps.Object, filter);
            var message = TestMessages.GetTestDeleteSampleEntityMesssage();

            logicClass.RouteSampleMessage(message);

            dataOps.Verify(d => d.DeleteSampleEntities(It.IsAny<object>()), Times.Once());
        }

   

    }

 


}
