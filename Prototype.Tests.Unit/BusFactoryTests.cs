using System;
using System.Configuration;
using EasyNetQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factory;

namespace Prototype.Tests.Unit
{
    [TestClass]
    public class BusFactoryTests
    {
        [TestMethod]
        public void CreateMessageBusReturnsIBus()
        {
            var result = BusFactory.CreateMessageBus();

            Assert.IsInstanceOfType(result, typeof (IBus));
        }

    }
}
