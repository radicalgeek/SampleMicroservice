namespace Prototype.Service.Filters
{
    public interface IMessageFilter
    {
        bool ShouldTryProcessingMessage(dynamic message);
    }
}