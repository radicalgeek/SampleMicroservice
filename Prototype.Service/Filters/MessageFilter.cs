using System;
using Prototype.Logger;
using Prototype.Service.Settings;

namespace Prototype.Service.Filters
{
    public class MessageFilter : IMessageFilter
    {
        private readonly IEnvironment _environment;
        private readonly ILogger _logger;
        public MessageFilter(IEnvironment environment, ILogger logger)
        {
            _environment = environment;
            _logger = logger;
        }
        public bool ShouldTryProcessingMessage(dynamic message)
        {
            var servicename = _environment.GetServiceName();
            
            if (!CheckMessageIfForThisService(message, servicename)) return false;
            if (!CheckForVersionCompatiblity(message, servicename)) return false;
            if (!CheckForSatisfiableNeed(message)) return false;
            if (!CheckForExistingSolutions(message)) return false;
            return true;
        }

        private bool CheckMessageIfForThisService(dynamic message, string servicename)
        {
            if (servicename != message.ModifiedBy) return true;
            _logger.Info("Event=\"Message Dropped\" Message=\"This message was droped as it was last modified by this service\" ");
            return false;
        }

        private bool CheckForVersionCompatiblity(dynamic message, string servicename)
        {
            var currentMajorVersion = _environment.GetServiceVersion();
            try
            {
                foreach (var versionRequirement in message.CompatibleServiceVersions)
                {
                    if (versionRequirement.Service == servicename)
                    {
                        if (versionRequirement.Version > currentMajorVersion)
                        {
                            _logger.Info("Event=\"Message Dropped\" Message=\"This message was droped as the service version is not compatible with the message requirements\" RequiredVersion=\"{0}\" CurrentVersion=\"{1}\" ", currentMajorVersion, versionRequirement.Version);
                            return false;
                        }                      
                    }
                }
            }
            catch (Exception)
            {

            }
            return true;
        }

        private bool CheckForSatisfiableNeed(dynamic message)
        {
            if (CheckIfCreateOpperation(message)) return true;

            var satisfiableNeedFound = false;
            if (message.Needs != null && message.Needs.Count > 0)
            {
                foreach (var need in message.Needs)
                {
                    try
                    {
                        var sampleId = need.SampleUuid;
                        satisfiableNeedFound = true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (!satisfiableNeedFound)
            {
                _logger.Info("Event=\"Message Dropped\" Message=\"This message was droped as it contains no Needs satisfiable by this serivice\"");
                return false;
            }
               
            return true;
        }

        private static bool CheckIfCreateOpperation(dynamic message)
        {
            if (message.Method == "POST")
                return true;
            return false;
        }

        private bool CheckForExistingSolutions(dynamic message)
        {
            var solutionExists = false;
            if (message.Solutions != null && message.Solutions.Count > 0)
            {
                foreach (var solution in message.Solutions)
                {
                    try
                    {
                        var sampleId = solution.SampleUuid;
                        solutionExists = true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (solutionExists)
            {
                _logger.Info("Event=\"Message Dropped\" Message=\"This message was droped as it already contains a Solution satisfiable by this serivice\"");
                return false;
            }
                
            return true;
        }
    }
}
