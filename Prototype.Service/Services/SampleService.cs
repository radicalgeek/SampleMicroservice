using System.Reflection;
using Ninject;
using Prototype.Logger;
using Prototype.Subscribers.Startables;
using Topshelf;
using Topshelf.Logging;

namespace Prototype.Service
{
    /// <summary>
    /// Sample service to demonstrate topshelf microservice
    /// </summary>
    public class SampleService : ServiceControl, ISampleService
    {
        private readonly ILogger _logger;
        private IAutoSubscriber _sampleAutoSubscriber;

        public SampleService(ILogger logger, IAutoSubscriber sampleAutoSubscriber)
        {
            _logger = logger;
            _sampleAutoSubscriber = sampleAutoSubscriber;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="hostControl">the topshelf host</param>
        /// <returns>always returns true</returns>
        public bool Start(HostControl hostControl)
        {
            _sampleAutoSubscriber.Start();

            _logger.Info("Prototype .NET Micro Service Started");
            return true;
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <param name="hostControl">the topshelf host</param>
        /// <returns>always returns true</returns>
        public bool Stop(HostControl hostControl)
        {
            _sampleAutoSubscriber.Stop();
            _logger.Info("Prototype .NET Micro Service Stopped");
            return true;
        }
    }
}