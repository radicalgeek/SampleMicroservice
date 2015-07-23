using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Prototype.Infrastructure
{
    /// <summary>
    /// This class retrives values from environment variables. If the requested variable is not found, then it will be
    /// pulled from the app config and set. This should allow the application to work with docker environment variables
    /// when run in a container, or app settings when run nativly in windows or whilst debugging or testing :-D
    /// </summary>
    public static class HostingEnvironment
    {
        /// <summary>
        /// Retrive an environment variable. If the variable dosn't exist it will be created and populated from the app config
        /// </summary>
        /// <param name="requestedVariable">the Variable name</param>
        /// <returns>Environment Variable</returns>
        public static string GetEnvironmentVariable(string requestedVariable)
        {
            var value = Environment.GetEnvironmentVariable(requestedVariable);
            if (value == null)
            {
                Environment.SetEnvironmentVariable(requestedVariable, ConfigurationManager.AppSettings[requestedVariable]);
                value = Environment.GetEnvironmentVariable(requestedVariable);
            }
            return value;
        }
    }
}
