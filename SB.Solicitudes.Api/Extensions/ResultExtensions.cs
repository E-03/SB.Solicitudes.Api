using Microsoft.AspNetCore.Mvc;
using SB.Solicitudes.Application.Common.Models;

namespace SB.Solicitudes.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ResultEntity<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return BuildErrorResult(result.Errors);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }

            return BuildErrorResult(result.Errors);
        }

        private static IActionResult BuildErrorResult(
            IReadOnlyCollection<ResultError> errors)
        {
            var type = errors.First().Type;

            return type switch
            {
                ErrorType.Validation => new BadRequestObjectResult(errors),

                ErrorType.NotFound => new NotFoundObjectResult(errors),

                ErrorType.Unauthorized => new UnauthorizedObjectResult(errors),

                ErrorType.Forbidden => new ObjectResult(errors)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                },

                ErrorType.Conflict => new ConflictObjectResult(errors),

                _ => new ObjectResult(errors)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }
    }
}
