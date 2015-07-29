using System;
using System.Configuration;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using Prototype.Infrastructure;
using Prototype.Infrastructure.Factory;

namespace Prototype.Tests.Unit
{
    [TestFixture]
    public class BusFactoryTests
    {
        [Test]
        public void CreateMessageBusReturnsIBus()
        {
            var result = BusFactory.CreateMessageBus();

            Assert.IsInstanceOfType(typeof(IBus), result);
        }

    }
}
