using Auth.Infrastructure.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Auth.Infrastructure.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate? _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            List<string> result = new List<string>();
            IDictionary<string, string[]> errors = GetErrors(exception);
            if (errors != null)
            {
                foreach (var item in ((ValidationException)exception).Failures)
                {
                    foreach (var str in item.Value.ToList())
                    {
                        Serilog.Log.Error(str);
                        result.Add(str);
                    }
                }
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(exception);

            if (result.Count <= 0)
            {
                result.Add(exception.Message);
            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                );
        }
        private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

        private static IDictionary<string, string[]> GetErrors(Exception exception)
        {
            IDictionary<string, string[]> errors = null;
            if (exception is ValidationException validationException)
            {
                errors = validationException.Failures;
            }
            return errors;
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
