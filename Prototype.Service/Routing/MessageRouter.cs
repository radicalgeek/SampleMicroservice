using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using MongoRepository;
using Prototype.Infrastructure.Settings;
using Prototype.Logger;
using Prototype.Service.Data;
using Prototype.Service.Data.Model;
using Prototype.Service.Publish;

namespace Prototype.Service.Routing
{
    /// <summary>
    /// This class proccesses incoming messages, and persists them to the data store
    /// </summary>
    public class MessageRouter : IMessageRouter
    {
        
        private readonly IEnvironment _environment;
        private readonly IDataOperations _dataOperations;

        public MessageRouter(IEnvironment environment, IDataOperations dataOperations)
        {
            _environment = environment;
            _dataOperations = dataOperations;
        }

        /// <summary>
        /// Route the message based on the "method" field in the incoming message.
        /// </summary>
        /// <param name="message">dynamic message object from the bus keeps contracts loosly coupled</param>
        public void RouteSampleMessage(dynamic message)
        {
            if (ShouldTryProcessingMessage(message))
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

        public bool ShouldTryProcessingMessage(dynamic message)
        {
            var servicename = _environment.GetServiceName();
            int currentMajorVersion = _environment.GetServiceVersion();

            if (servicename == message.ModifiedBy)
                return false;

            try
            {
                foreach (var versionRequirement in message.CompatibleServiceVersions)
                {
                    if (versionRequirement.Service == servicename)
                    {
                        if (versionRequirement.Version > currentMajorVersion)
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }

      
    }
}
