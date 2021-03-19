using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Dtos
{
    public class CustomerInputDto
    {
        public string Name { get; set; }
    }

    public class CustomerOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<CustomerOutputDto> GetAll()
        {
            for (int i = 1; i <= 100; i++)
                yield return new CustomerOutputDto { Id = i, Name = $"Customer {i}" };
        }
    }

    public class PagedResult<TResult>
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public IEnumerable<TResult> Results { get; set; }
    }
}
