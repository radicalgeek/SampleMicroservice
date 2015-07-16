using Prototype.MessageTypes.Messages;

namespace Prototype.Logic
{
    /// <summary>
    /// Interface used for Dependancy Injection of the logic layer into the service class
    /// </summary>
    public interface ISampleLogic
    {
        void RouteSampleMessage(dynamic message);

    }
}