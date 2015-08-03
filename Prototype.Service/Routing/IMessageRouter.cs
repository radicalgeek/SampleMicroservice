namespace Prototype.Service.Routing
{
    /// <summary>
    /// Interface used for Dependancy Injection of the logic layer into the service class
    /// </summary>
    public interface IMessageRouter
    {
        void RouteSampleMessage(dynamic message);

    }
}