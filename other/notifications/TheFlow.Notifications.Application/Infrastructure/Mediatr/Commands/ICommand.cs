﻿using MediatR;

namespace TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands
{
    public interface ICommand<out T> : IRequest<T>
    {
    }

    public interface ICommand : ICommand<bool>
    {
    }
}
