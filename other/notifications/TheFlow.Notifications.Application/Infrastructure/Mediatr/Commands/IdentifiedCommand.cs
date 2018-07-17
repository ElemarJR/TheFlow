﻿using System;
using MediatR;

namespace TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands
{
    public class IdentifiedCommand<TCommand, TResult> :
        IRequest<TResult>
        where TCommand : IRequest<TResult>
    {
        public Guid Id { get; }
        public TCommand Command { get; }

        public IdentifiedCommand(TCommand command, Guid id)
        {
            Id = id;
            Command = command;
        }
    }
}
