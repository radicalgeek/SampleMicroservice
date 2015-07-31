namespace Prototype.Service.Subscribe
{
    public interface IMessageSubscriber
    {
        void Start();
        void Stop();
    }
}