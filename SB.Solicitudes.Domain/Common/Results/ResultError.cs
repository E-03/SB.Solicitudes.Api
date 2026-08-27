using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Common.Results
{
    public sealed record ResultError(
        string Code,
        string Message,
        ErrorType Type,
        string? PropertyName = null)
    {
        public static ResultError Validation(
            string code,
            string message,
            string? propertyName = null)
            => new(
                code,
                message,
                ErrorType.Validation,
                propertyName);

        public static ResultError NotFound(
            string code,
            string message)
            => new(
                code,
                message,
                ErrorType.NotFound);

        public static ResultError Conflict(
            string code,
            string message)
            => new(
                code,
                message,
                ErrorType.Conflict);

        public static ResultError Unauthorized(
            string code,
            string message)
            => new(
                code,
                message,
                ErrorType.Unauthorized);

        public static ResultError Forbidden(
            string code,
            string message)
            => new(
                code,
                message,
                ErrorType.Forbidden);

        public static ResultError Failure(
            string code,
            string message)
            => new(
                code,
                message,
                ErrorType.Failure);
    }
}
