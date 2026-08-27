namespace SB.Solicitudes.Application.Common.Models
{
    public sealed class ResultEntity<T> : Result
    {
        public T? Value { get; }

        private ResultEntity(T value)
        {
            Value = value;
        }

        private ResultEntity(IEnumerable<ResultError> errors)
            : base(errors)
        {
        }

        public static ResultEntity<T> Success(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return new ResultEntity<T>(value);
        }

        public static new ResultEntity<T> Failure(ResultError error)
            => new([error]);

        public static new ResultEntity<T> Failure(IEnumerable<ResultError> errors)
            => new(errors);
    }
}
