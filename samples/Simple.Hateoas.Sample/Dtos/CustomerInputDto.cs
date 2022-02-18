using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Dtos
{
    public class CustomerInputDto
    {
        public string Name { get; set; }
        public IEnumerable<string> PhoneNumbers { get; set; }
    }
}
