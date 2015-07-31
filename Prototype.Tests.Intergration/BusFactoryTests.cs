using System;
using EasyNetQ;
using NUnit.Framework;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factories;
using Moq;

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
