using Newtonsoft.Json;
using Serilog;

namespace Application.Common.ResultsModel
{
    public class Result
    {
        public Result() { }

        internal Result(bool succeeded, string message, object entity = default, string exception = null)
        {
            Succeeded = succeeded;
            Message = message;
            ExceptionError = exception;
            Entity = entity;
        }

        internal Result(bool succeeded, object entity = default)
        {
            Succeeded = succeeded;
            Entity = entity;
        }

        public bool Succeeded { get; set; }
        public object Entity { get; set; }
        public string Error { get; set; }
        public string ExceptionError { get; set; }
        public string Message { get; set; }

        public static Result Success(object entity)
        {
            Log.Information("Request was executed successfully!");
            return new Result(true, "Request was executed successfully!", entity);
        }

        public static Result Success(Type request, string message)
        {
            Log.Information(message);
            return new Result(true, message);
        }

        public static Result Success(object entity, string message, Type request)
        {
            Log.Information(message, entity);
            return new Result(true, message, entity);
        }

        public static Result Success(object entity, Type request)
        {
            Log.Information("Request was executed successfully!", entity);
            return new Result(true, entity);
        }

        public static Result Success(string message)
        {
            Log.Information(message);
            return new Result(true, message);
        }

        public static Result Success<T>(string message, object entity)
        {
            Log.Information($"{message} {Environment.NewLine}" +
                $" {JsonConvert.SerializeObject(entity, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return new Result(true, message, entity);
        }

        public static Result Success<T>(object entity, T request)
        {
            Log.Information("Request was executed successfully!");
            return new Result(true, entity);
        }
       
        public static Result Success<T>(object entity, T request, string message)
        {
            Log.Information(message);
            return new Result(true, message, entity);
        }

        public static Result Success<T>(object entity, string message, T request)
        {
            Log.Information(message);
            return new Result(true, message, entity);
        }

        public static Result Failure(Type request, string error)
        {
            Log.Error(error);
            return new Result(false, error);
        }

        public static Result Failure(Type request, string prefixMessage, Exception ex)
        {
            Log.Error($"{prefixMessage} Error: {ex?.Message + Environment.NewLine + ex?.InnerException?.Message}");
            return new Result(false, $"{prefixMessage} Error: {ex?.Message + Environment.NewLine + ex?.InnerException?.Message}");
        }

        public static Result Failure<T>(string error)
        {
            Log.Error(error);
            return new Result(false, error);
        }
        
        public static Result Failure(string error, Exception ex )
        {
            Log.Error(error);
            return new Result(false, error);
        }

        public static Result Failure<T>(T request, string error)
        {
            Log.Error(error);
            return new Result(false, error);
        }

        public static Result Failure<T>(string prefixMessage, Exception ex)
        {
            Log.Error($"{prefixMessage} Error: {ex?.Message + Environment.NewLine + ex?.InnerException?.Message}");
            return new Result(false, $"{prefixMessage}");
        }

        public static Result Failure<T>(T request, string prefixMessage, Exception ex)
        {
            Log.Error($"{prefixMessage} Error: {ex?.Message + Environment.NewLine + ex?.InnerException?.Message}");
            return new Result(false, $"{prefixMessage}");
        }

        public static Result Failure<T>(T request, object entity)
        {
            Log.Error($"Error: {DateTime.Now}");
            return new Result(false, entity);
        }

        public static Result Failure(Type request, object entity)
        {
            Log.Error($"Error: {DateTime.Now}");
            return new Result(false, entity);
        }

        public static Result Failure<T>(object entity)
        {
            Log.Error($"Error: {DateTime.Now}");
            return new Result(false, entity);
        }

        public static Result Info(Type request, string information)
        {
            Log.Information(information);
            return new Result(true, information);
        }

        public static Result Info(Type request, object entity)
        {
            Log.Information("Information!");
            return new Result(true, entity);
        }

        public static Result Info<T>(T request, string message)
        {
            Log.Information(message, DateTime.Now);
            return new Result(true, message);
        }

        public static Result Info<T>(T request, object entity)
        {
            Log.Information("Information");
            return new Result(true, entity);
        }

        public static Result Warning(Type request, string message)
        {
            Log.Warning(message);
            return new Result(false, message);
        }

        public static Result Warning(Type request, object entity)
        {
            Log.Warning("Warning!", entity);
            return new Result(false, entity);
        }

        public static Result Warning<T>(string message)
        {
            Log.Warning(message);
            return new Result(false, message);
        }

        public static Result Warning<T>(object entity)
        {
            Log.Warning("Warning!");
            return new Result(false, entity);
        }

        public static Result Warning<T>(T request, string message)
        {
            Log.Warning(message);
            return new Result(false, message);
        }

        public static Result Warning<T>(T request, object entity)
        {
            Log.Warning("Warning!");
            return new Result(false, entity);
        }

        public static Result Critical(Type request, string message)
        {
            Log.Fatal(message);
            return new Result(false, message);
        }

        public static Result Critical(Type request, object entity)
        {
            Log.Fatal("Warning!", entity);
            return new Result(false, entity);
        }

        public static Result Critical<T>(string message)
        {
            Log.Fatal(message);
            return new Result(false, message);
        }

        public static Result Critical<T>(object entity)
        {
            Log.Fatal("Warning!");
            return new Result(false, entity);
        }

        public static Result Critical<T>(T request, string message)
        {
            Log.Fatal(message);
            return new Result(false, message);
        }

        public static Result Critical<T>(T request, object entity)
        {
            Log.Fatal("Critical!");
            return new Result(false, entity);
        }

        public static Result Exception(Type request, Exception ex)
        {
            Log.Fatal(ex, "Exception:");
            return new Result(false, ex.Message, null, ex?.InnerException?.Message);
        }

        public static Result Exception<T>(T request, Exception ex)
        {
            Log.Fatal(ex, "Exception:");
            return new Result(false, ex.Message, default, ex?.InnerException?.Message);
        }
    }
}
