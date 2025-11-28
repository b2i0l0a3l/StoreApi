using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.EventBus
{
    public class MediatRPublisher : IEventBus
    {
        private readonly IMediator _mediator;

        public MediatRPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : INotification
        {
            await _mediator.Publish(@event);
        }
    }
}