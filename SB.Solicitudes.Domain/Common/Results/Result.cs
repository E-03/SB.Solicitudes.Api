using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Common.Results
{
    public class Result
    {
        private readonly List<ResultError> _errors = [];

        public bool IsSuccess =>
            _errors.Count == 0;

        public bool IsFailure =>
            !IsSuccess;

        public IReadOnlyCollection<ResultError> Errors =>
            _errors.AsReadOnly();

        protected Result()
        {
        }

        protected Result(
            IEnumerable<ResultError> errors)
        {
            _errors.AddRange(errors);
        }

        public static Result Success()
            => new();

        public static Result Failure(
            ResultError error)
            => new([error]);

        public static Result Failure(
            IEnumerable<ResultError> errors)
            => new(errors);
    }
}
