using Prototype.Logger;
using Prototype.Service.Subscribe;
using Topshelf;

namespace Prototype.Service.Services
{
    /// <summary>
    /// Sample service to demonstrate topshelf microservice
    /// </summary>
    public class SampleService : ServiceControl, ISampleService
    {
        private readonly ILogger _logger;
        private readonly IMessageSubscriber _sampleMessageSubscriber;

        public SampleService(ILogger logger, IMessageSubscriber sampleMessageSubscriber)
        {
            _logger = logger;
            _sampleMessageSubscriber = sampleMessageSubscriber;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        /// <param name="hostControl">the topshelf host</param>
        /// <returns>always returns true</returns>
        public bool Start(HostControl hostControl)
        {
            _sampleMessageSubscriber.Start();

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
            _sampleMessageSubscriber.Stop();
            _logger.Info("Prototype .NET Micro Service Stopped");
            return true;
        }
    }
}