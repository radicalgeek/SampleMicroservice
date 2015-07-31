using Prototype.MessageTypes.Messages;

namespace Prototype.Logic
{
    /// <summary>
    /// Interface used for Dependancy Injection of the logic layer into the service class
    /// </summary>
    public interface IServiceLogic
    {
        void RouteSampleMessage(dynamic message);

    }
}