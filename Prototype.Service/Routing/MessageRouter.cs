using System;
using Prototype.Logger;
using Prototype.Service.Data;
using Prototype.Service.Filters;
using IEnvironment = Prototype.Service.Settings.IEnvironment;

namespace Prototype.Service.Routing
{
    /// <summary>
    /// This class proccesses incoming messages, and persists them to the data store
    /// </summary>
    public class MessageRouter : IMessageRouter
    {
        
        private readonly ILogger _logger;
        private readonly IDataOperations _dataOperations;
        private readonly IMessageFilter _filter;

        public MessageRouter(ILogger logger, IDataOperations dataOperations, IMessageFilter filter)
        {
            _logger = logger;
            _dataOperations = dataOperations;
            _filter = filter;
        }

        /// <summary>
        /// Route the message based on the "method" field in the incoming message.
        /// </summary>
        /// <param name="message">dynamic message object from the bus keeps contracts loosly coupled</param>
        public void RouteSampleMessage(dynamic message)
        {
            if (_filter.ShouldTryProcessingMessage(message))
            {
                switch ((string) message.Method.ToString())
                {
                    case "GET":
                        _logger.Info("Event=\"Message Routed\" Operation=\"GET\" ");
                        _dataOperations.GetSampleEntities(message);
                        break;
                    case "POST":
                        _logger.Info("Event=\"Message Routed\" Operation=\"POST\" ");
                        _dataOperations.CreateSampleEntities(message);
                        break;
                    case "PUT":
                        _logger.Info("Event=\"Message Routed\" Operation=\"PUT\" ");
                        _dataOperations.UpdateSampleEntities(message);
                        break;
                    case "DELETE":
                        _logger.Info("Event=\"Message Routed\" Operation=\"DELETE\" ");
                        _dataOperations.DeleteSampleEntities(message);
                        break;
                }
            }
        }

        

      
    }
}
