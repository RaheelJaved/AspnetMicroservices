using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                string validationErrorDetails = string.Empty;
                if (ex is ValidationException) {
                    StringBuilder allErrors = new StringBuilder();

                    foreach (var error in ((ValidationException)ex).Errors)
                    {
                        allErrors.AppendLine($"{error.Key}: {String.Join(" | ", error.Value)}");
                    }
                    validationErrorDetails = allErrors.ToString();
                }
                _logger.LogError(ex, "Application Request: Unhandled Exception for Request {requestName} {request}. {validationErrorDetails}",requestName, request, validationErrorDetails);
                throw;
            }
        }
    }
}