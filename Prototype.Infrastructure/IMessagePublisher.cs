using System;

namespace Prototype.Infrastructure
{
    public interface IMessagePublisher
    {
        void Publish(dynamic message, string topic);

    }
}