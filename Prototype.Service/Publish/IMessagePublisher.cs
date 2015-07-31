namespace Prototype.Service.Publish
{
    public interface IMessagePublisher
    {
        void Publish(dynamic message, string topic);

    }
}