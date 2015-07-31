namespace Prototype.Service.Routing
{
    /// <summary>
    /// Interface used for Dependancy Injection of the logic layer into the service class
    /// </summary>
    public interface IServiceLogic
    {
        void RouteSampleMessage(dynamic message);

    }
}