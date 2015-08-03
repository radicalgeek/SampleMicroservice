using System;
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
        
        private readonly IEnvironment _environment;
        private readonly IDataOperations _dataOperations;
        private readonly IMessageFilter _filter;

        public MessageRouter(IEnvironment environment, IDataOperations dataOperations, IMessageFilter filter)
        {
            _environment = environment;
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
                        _dataOperations.GetSampleEntities(message);
                        break;
                    case "POST":
                        _dataOperations.CreateSampleEntities(message);
                        break;
                    case "PUT":
                        _dataOperations.UpdateSampleEntities(message);
                        break;
                    case "DELETE":
                        _dataOperations.DeleteSampleEntities(message);
                        break;
                }
            }
        }

        

      
    }
}
