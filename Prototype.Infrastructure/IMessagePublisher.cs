using System;

namespace Prototype.Infrastructure
{
    public interface IMessagePublisher
    {
        void Publish(dynamic message);

        void Request<TRequest, TResponse>(TRequest request, Action<TResponse> onResponse)
            where TRequest : class
            where TResponse : class;

        void Response<TRequest, TResponse>(Func<TRequest, TResponse> onResponse)
            where TRequest : class
            where TResponse : class;
    }
}