using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Core.Dtos
{
    public class CustomerInputDto
    {
        public string Name { get; set; }
        public IEnumerable<string> PhoneNumbers { get; set; }
    }
}
