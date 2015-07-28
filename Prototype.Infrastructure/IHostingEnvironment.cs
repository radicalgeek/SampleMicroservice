namespace Prototype.Infrastructure
{
    public interface IHostingEnvironment
    {
        /// <summary>
        /// Retrive an environment variable. If the variable dosn't exist it will be created and populated from the app config
        /// </summary>
        /// <param name="requestedVariable">the Variable name</param>
        /// <returns>Environment Variable</returns>
        string GetEnvironmentVariable(string requestedVariable);

        int GetServiceVersion();
        string GetServiceName();
    }
}