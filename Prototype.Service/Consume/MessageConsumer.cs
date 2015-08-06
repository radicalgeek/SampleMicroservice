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
        private readonly IMessageRouter _messageRouter;

        public MessageConsumer(ILogger logger, IMessageRouter messageRouter)
        {
            _logger = logger;
            _messageRouter = messageRouter;
        }

        /// <summary>
        /// Consumes incoming messages, first converting the message JSON into a dynamic object, #
        /// them passing it to a business logic layer
        /// </summary>
        /// <param name="message">Sample message from bus with JSON string</param>
        public void Consume(dynamic message)
        {
            var stopwatch = GetStopwatch();
            _logger.Info("Event=\"Begun Consuming Message\" CorrelationId=\"{0}\" ", message.Properties.CorrelationId);
            var dynamicMessageObject = GetDynamicMessageObject(message);

            try
            {
                _messageRouter.RouteSampleMessage(dynamicMessageObject);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Event=\"Error Consuming Message\" CorrelationId=\"{0}\" ", message.Properties.CorrelationId);
            }
            stopwatch.Stop();
            _logger.Trace("Event=\"Finished Consuming Message\" CorrelationId=\"{0}\" TimeElapsed=\"{1}\"", message.Properties.CorrelationId, stopwatch.Elapsed);

        }

        /// <summary>
        /// Converts incoming JSON messagges to C# 4 dynamic objects
        /// </summary>
        /// <param name="message">the incoming SampleMessage Object (with a JSON string)</param>
        /// <returns>dynamic object</returns>
        private dynamic GetDynamicMessageObject(dynamic message)
        {
            try
            {
                return JsonConvert.SerializeObject(message.Body);   
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Event=\"ErrorConsumingMessage\" CorrelationId=\"{0}\" Message=\"Unable to convert message to C# dynamic object\" ", message.Properties.CorrelationId);
                throw;
            }
                    
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