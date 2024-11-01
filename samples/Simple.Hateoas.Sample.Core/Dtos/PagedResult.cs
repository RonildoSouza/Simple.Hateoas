using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Core.Dtos
{
    public class PagedResult<TResult>
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public IEnumerable<TResult> Results { get; set; }
    }
}
