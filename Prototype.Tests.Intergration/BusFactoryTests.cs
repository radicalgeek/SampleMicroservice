using System;
using EasyNetQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factory;
using Moq;

namespace Prototype.Tests.Intergration
{
    [TestClass]
    public class BusFactoryTests
    {
        [TestMethod]
        public void CreateMessageBusReturnsConnectedBus()
        {

            var result = BusFactory.CreateMessageBus();
            
            Assert.IsTrue(result.IsConnected);
        }
    }
}
