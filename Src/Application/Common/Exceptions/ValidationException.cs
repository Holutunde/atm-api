using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        IEnumerable<IGrouping<string, string>> failureGroups = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

        foreach (IGrouping<string, string> failureGroup in failureGroups)
        {
            string propertyName = failureGroup.Key;
            string[] propertyFailures = failureGroup.ToArray();

            Errors.Add(propertyName, propertyFailures);
        }
    }

    public IDictionary<string, string[]> Errors { get; }

    public string GetErrors()
    {
        string errors = string.Empty;
        try
        {
            if (Errors.Any())
            {
                
                foreach (KeyValuePair<string, string[]> error in Errors)
                {
                    _ = string.Concat(",", error);
                }
            }
            return errors.TrimEnd(';');
        }
        catch (Exception ){ return errors; }
    }
}