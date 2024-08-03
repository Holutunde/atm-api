using Serilog;
using System.Text.Json;
using Application.Common.ResultsModel;

namespace API.Filters
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception error)
            {
                Result responseModel;
                HttpResponse response = httpContext.Response;
                response.ContentType = "application/json";
                string msg = "An error occured, please try again later!";
                switch (error)
                {
                    case UnauthorizedAccessException _:
                        msg = "You are not authorized!";
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;

                    default:
                        Log.Error(error.ToString());
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }
                responseModel = Result.Failure<string>(msg);
                string result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await response.WriteAsync(result);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }


}