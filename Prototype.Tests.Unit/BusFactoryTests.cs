﻿using System;
using System.Configuration;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using Prototype.Service.Factories;

namespace Prototype.Tests.Unit
{
    [TestFixture]
    public class BusFactoryTests
    {
        [Test]
        public void CreateMessageBusReturnsIAdvancedBus()
        {
            var result = BusFactory.CreateMessageBus();

            Assert.IsInstanceOfType(typeof(IAdvancedBus), result);
        }

    }
}
