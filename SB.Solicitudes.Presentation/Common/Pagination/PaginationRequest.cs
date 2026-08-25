namespace SB.Solicitudes.Application.Common.Pagination
{
    public sealed class PaginationRequest
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;

        public int PageNumber { get; init; } = DefaultPageNumber;

        public int PageSize { get; init; } = DefaultPageSize;

        public int ValidPageNumber =>
            PageNumber < 1
                ? DefaultPageNumber
                : PageNumber;

        public int ValidPageSize =>
            PageSize < 1
                ? DefaultPageSize
                : Math.Min(PageSize, MaxPageSize);

        public int Skip =>
            (ValidPageNumber - 1) * ValidPageSize;
    }
}