using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Core.Dtos
{
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
}
