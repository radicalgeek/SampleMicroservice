using System;
using EasyNetQ;
using NUnit.Framework;
using Moq;
using Prototype.Service.Factories;

namespace Prototype.Tests.Intergration
{
    [TestFixture]
    public class BusFactoryTests
    {
        [Test]
        public void CreateMessageBusReturnsConnectedBus()
        {
            var result = BusFactory.CreateMessageBus();        
            Assert.IsTrue(result.IsConnected);
        }
    }
}
