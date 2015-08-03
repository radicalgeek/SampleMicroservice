using System;
using System.Configuration;
using System.Diagnostics;

namespace Prototype.Service.Settings
{
    /// <summary>
    /// This class retrives values from environment variables. If the requested variable is not found, then it will be
    /// pulled from the app config and set. This should allow the application to work with docker environment variables
    /// when run in a container, or app settings when run nativly in windows or whilst debugging or testing :-D
    /// </summary>
    public class Environment : IEnvironment
    {
        /// <summary>
        /// Retrive an environment variable. If the variable dosn't exist it will be created and populated from the app config
        /// </summary>
        /// <param name="requestedVariable">the Variable name</param>
        /// <returns>Environment Variable</returns>
        public string GetEnvironmentVariable(string requestedVariable)
        {
            var value = System.Environment.GetEnvironmentVariable(requestedVariable);
            if (value == null)
            {
                System.Environment.SetEnvironmentVariable(requestedVariable, ConfigurationManager.AppSettings[requestedVariable]);
                value = System.Environment.GetEnvironmentVariable(requestedVariable);
            }
            return value;
        }

        public int GetServiceVersion()
        {
            string fullExeNameAndPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Convert.ToInt32(FileVersionInfo.GetVersionInfo(fullExeNameAndPath).ProductVersion.Split('.')[0]);
        }

        public string GetServiceName()
        {
            string fullExeNameAndPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetFileName(fullExeNameAndPath);
        }
    }
}
