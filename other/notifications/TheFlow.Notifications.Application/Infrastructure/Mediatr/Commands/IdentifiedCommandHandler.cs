using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheFlow.Notifications.Application.Infrastructure.Idempotency;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Exceptions;

namespace TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands
{
    public class IdentifiedCommandHandler<TCommand, TResult>
        : IRequestHandler<IdentifiedCommand<TCommand, TResult>, TResult>
        where TCommand : IRequest<TResult>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager)
        {
            _mediator = mediator;
            _requestManager = requestManager;
        }

        public Task<TResult> Handle(
            IdentifiedCommand<TCommand, TResult> message
        )
        {
            return Handle(message, new CancellationToken());
        }

        public async Task<TResult> Handle(
            IdentifiedCommand<TCommand, TResult> message,
            CancellationToken cancellationToken
        )
        {
            if (message.Id == Guid.Empty)
            {
                ThrowMediatrPipelineException.IdentifiedCommandWithoutId();
            }

            if (message.Command == null)
            {
                ThrowMediatrPipelineException.IdentifiedCommandWithoutInnerCommand();
            }

            var alreadyRegistered = await _requestManager.IsRegistered(message.Id, cancellationToken);
            if (alreadyRegistered)
            {
                ThrowMediatrPipelineException.CommandWasAlreadyExecuted();
            }

            await _requestManager.Register(message.Id, cancellationToken);
            var result = await _mediator.Send(message.Command, cancellationToken);
            return result;
        }
    }
}
