using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{

    //https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-6.0
    public static class InvalidModelStateLogger
    {
        public static Action<ApiBehaviorOptions> Configure => (options) =>
        {
            // To preserve the default behavior, capture the original delegate to call later.
            var builtInFactory = options.InvalidModelStateResponseFactory;

            options.InvalidModelStateResponseFactory = context =>
            {



                //https://github.com/dotnet/AspNetCore.Docs/issues/12157#issuecomment-487756787
                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);

                // Perform logging here.
                //logger.LogError("{InvalidModelState}", context.ModelState.AllValidationErrors());

                context.ModelState.LogValidationErrors(logger);

                // Invoke the default behavior, which produces a ValidationProblemDetails
                // response.
                // To produce a custom response, return a different implementation of 
                // IActionResult instead.
                return builtInFactory(context);
            };
        };

        private static void LogValidationErrors (this ModelStateDictionary modelStateDictionary, ILogger logger)
        {  
            StringBuilder allErrors = new StringBuilder();
            foreach (var validationError in modelStateDictionary)
            {
                allErrors.AppendLine($"{validationError.Key}: {String.Join(" | ", validationError.Value.Errors.Select(err => err.ErrorMessage))}");
            }
            logger.LogInformation("{modelValidationErrorsCount} model validation errors occured: {allModelValidationErrors}", modelStateDictionary.ErrorCount, allErrors);
        }
    }
}
