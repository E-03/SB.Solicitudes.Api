using FluentValidation.Results;
using SB.Solicitudes.Application.Common.Models;

namespace SB.Solicitudes.Application.Common.Extensions
{
    public static class ValidationResultExtensions
    {
        public static IReadOnlyCollection<ResultError> ToResultErrors(
            this ValidationResult validationResult)
        {
            return validationResult.Errors
                .Select(e => ResultError.Validation(
                    $"Validation.{e.PropertyName}",
                    e.ErrorMessage,
                    e.PropertyName))
                .ToList();
        }
    }
}
