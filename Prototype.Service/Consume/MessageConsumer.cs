using System;
using System.Diagnostics;
using EasyNetQ;
using Newtonsoft.Json;
using Prototype.Logger;
using Prototype.Service.Messages;
using Prototype.Service.Routing;

namespace Prototype.Service.Consume
{
    /// <summary>
    /// This is a sample consumer of Test Messages. The autosubscriber automaticly registers any class that impliments
    /// the IConsume interface, using the type parameter to determine the contract to subscribe to. In this implimentation
    /// the contract simply contains a string for JSON data that is later cast to a dynamic object. This means that this one 
    /// consumer can deal with all message types
    /// </summary>
    public class MessageConsumer : IMessageConsumer
    {
        private readonly ILogger _logger;
        private readonly IServiceLogic _serviceLogic;

        public MessageConsumer(ILogger logger, IServiceLogic serviceLogicLayer)
        {
            _logger = logger;
            _serviceLogic = serviceLogicLayer;
        }

        /// <summary>
        /// Consumes incoming messages, first converting the message JSON into a dynamic object, #
        /// them passing it to a business logic layer
        /// </summary>
        /// <param name="message">Sample message from bus with JSON string</param>
        public void Consume(IMessage<SampleMessage> message)
        {
            var stopwatch = GetStopwatch();

            _logger.Info("Message recieved {0}", message.Properties.CorrelationId);

            var dynamicMessageObject = GetDynamicMessageObject(message.Body);

            try
            {
                _logger.Info("Proccessing message {0} begun", message.Properties.CorrelationId);
                _serviceLogic.RouteSampleMessage(dynamicMessageObject);
                _logger.Info("Proccessing message {0} Succeded", message.Properties.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Processing message {0} failed", message.Properties.CorrelationId);
            }
            stopwatch.Stop();
            _logger.Trace("Message {0} proccessed in {1}", message.Properties.CorrelationId, stopwatch.Elapsed);

        }

        /// <summary>
        /// Converts incoming JSON messagges to C# 4 dynamic objects
        /// </summary>
        /// <param name="message">the incoming SampleMessage Object (with a JSON string)</param>
        /// <returns>dynamic object</returns>
        private static dynamic GetDynamicMessageObject(SampleMessage message)
        {
            return JsonConvert.SerializeObject(message);           
        }

        /// <summary>
        /// resets and returns the stop watch for the recording of execution time
        /// </summary>
        /// <returns>Stopwatch  object</returns>
        private  Stopwatch GetStopwatch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            return stopwatch;
        }
    }
}