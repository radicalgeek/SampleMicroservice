namespace Prototype.MessageTypes.Messages
{
    /// <summary>
    /// Sample Message object containing only a string (for JSON data) that will later be serialised
    /// to a dynamic object. This ensurs the contract remains loosly coupled, and new fields on the contract will not brake this service
    /// </summary>
    public class SampleMessage : IBusMessage 
    {
        public string Message { get; set; }
    }
}