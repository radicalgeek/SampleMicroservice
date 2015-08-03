using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Service.Settings;

namespace Prototype.Service.Filters
{
    public class MessageFilter : IMessageFilter
    {
        private IEnvironment _environment;
        public MessageFilter(IEnvironment environment)
        {
            _environment = environment;
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
            if (servicename == message.ModifiedBy)
                return false;
            return true;
        }

        private bool CheckForVersionCompatiblity(dynamic message, string servicename)
        {
            int currentMajorVersion = _environment.GetServiceVersion();
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
                return false;
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
                return false;
            return true;
        }
    }
}
