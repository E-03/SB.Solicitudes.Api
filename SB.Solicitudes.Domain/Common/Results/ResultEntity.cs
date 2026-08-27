using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Solicitudes.Domain.Common.Results
{
    public sealed class ResultEntity<T> : Result
    {
        public T? Value { get; }

        private ResultEntity(T value)
        {
            Value = value;
        }

        private ResultEntity(
            IEnumerable<ResultError> errors)
            : base(errors)
        {
        }

        public static ResultEntity<T> Success(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return new ResultEntity<T>(value);
        }

        public static ResultEntity<T> Failure(
            ResultError error)
            => new([error]);

        public static ResultEntity<T> Failure(
            IEnumerable<ResultError> errors)
            => new(errors);
    }
}
