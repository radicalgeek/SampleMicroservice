using System;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Prototype.Logger;
using EasyNetQ.AutoSubscribe;
using Newtonsoft.Json;
using Prototype.Logic;
using Prototype.MessageTypes.Messages;
using Prototype.Subscribers.Serializers;

namespace Prototype.Subscribers.Consumers
{
    /// <summary>
    /// This is a sample consumer of Test Messages. The autosubscriber automaticly registers any class that impliments
    /// the IConsume interface, using the type parameter to determine the contract to subscribe to. In this implimentation
    /// the contract simply contains a string for JSON data that is later cast to a dynamic object. This means that this one 
    /// consumer can deal with all message types
    /// </summary>
    public class SampleMessageConsumer : IConsume<SampleMessage>
    {
        private ILogger _logger;
        private ISampleLogic _sampleLogic;

        public SampleMessageConsumer(ILogger logger, ISampleLogic sampleLogicLayer)
        {
            _logger = logger;
            _sampleLogic = sampleLogicLayer;
        }


        public void Consume(SampleMessage message)
        {
            var stopwatch = GetStopwatch();

            _logger.Info("Message recieved {0}", message.Message);

            var dynamicMessageObject = GetDynamicMessageObject(message);

            try
            {
                _logger.Info("Proccessing message {0} begun", message.Message);
                _sampleLogic.RouteSampleMessage(dynamicMessageObject);
                _logger.Info("Proccessing message {0} Succeded", message.Message);
            }
            catch (Exception ex)
            {          
                _logger.Error(ex,"Processing message {0} failed", message.Message);
            }
            stopwatch.Stop();
            _logger.Trace("Message {0} proccessed in {1}", message.Message, stopwatch.Elapsed);

        }

        private static dynamic GetDynamicMessageObject(SampleMessage message)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] {new DynamicJsonConverter()});
            dynamic dynamicMessageObject = serializer.Deserialize(message.Message, typeof (object));
            return dynamicMessageObject;
        }

        private  Stopwatch GetStopwatch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            return stopwatch;
        }
    }
}