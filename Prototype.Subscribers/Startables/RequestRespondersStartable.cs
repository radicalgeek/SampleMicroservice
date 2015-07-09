using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Prototype.Subscribers.Responders;

namespace Prototype.Subscribers.Startables
{
    public class RequestRespondersStartable : IStartable
    {
        private readonly IWindsorContainer _container;

        public RequestRespondersStartable(IWindsorContainer container)
        {
            _container = container;
        }
        public void Start()
        {
            _container.Register(Classes.FromThisAssembly()
                                      .BasedOn<IResponder>()
                                      .WithServiceSelf()
                                      .WithServiceFromInterface());

            var repsonders = _container.ResolveAll<IResponder>();

            foreach (var responder in repsonders)
            {
                responder.Subscribe();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
