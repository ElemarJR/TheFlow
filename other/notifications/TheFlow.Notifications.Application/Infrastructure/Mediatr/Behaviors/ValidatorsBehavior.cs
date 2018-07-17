using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Exceptions;

namespace TheFlow.Notifications.Application.Infrastructure.Mediatr.Behaviors
{
    public class ValidatorsBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] _validators;

        public ValidatorsBehavior(IValidator<TRequest>[] validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var failures = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                throw new MediatrPipelineException(
                    $"Command Validation Errors for type {typeof(TRequest).Name}",
                    new ValidationException("Validation exception", failures)
                );
            }

            return await next();
        }
    }
}
