namespace Prototype.Service.Messages
{
    /// <summary>
    /// Interface to represent a standard bus message
    /// </summary>
    public interface IBusMessage
    {
        string Message { get; set; }
    }
}