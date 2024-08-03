using FluentValidation;
using FluentValidation.Results;


namespace Application.Extensions
{
    public static class ValidationExtension
    {
        public static async Task<ValidationResult> ValidateAsync<T>(this T request, IValidator<T> validator, CancellationToken cancellationToken = default)
        {
            return await validator.ValidateAsync(request, cancellationToken);
        }
    }
}