namespace BetterCRM.Core.Extensions
{
    public class PagedResult<T>
    {
        public List<T> Items { get; }
        public int Total { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
        public bool HasNext => Page < TotalPages;
        public bool HasPrev => Page > 1;

        public PagedResult(List<T> items, int total, int page, int pageSize)
        {
            Items = items;
            Total = total;
            Page = page;
            PageSize = pageSize;
        }
    }
}
