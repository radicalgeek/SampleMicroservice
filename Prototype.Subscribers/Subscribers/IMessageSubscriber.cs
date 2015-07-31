namespace Prototype.Messaging.Subscribers
{
    public interface IMessageSubscriber
    {
        void Start();
        void Stop();
    }
}