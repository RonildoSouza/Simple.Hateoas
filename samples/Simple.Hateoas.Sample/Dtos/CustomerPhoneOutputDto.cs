using System;
using System.Collections.Generic;

namespace Simple.Hateoas.Sample.Dtos
{
    public class CustomerPhoneOutputDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }

        public static IEnumerable<CustomerPhoneOutputDto> GetAll(int customerId)
        {
            var totalPhoneNumbers = new Random().Next(2, 6);

            for (int i = 1; i <= totalPhoneNumbers; i++)
            {
                var _ = customerId; // Fake where
                yield return new CustomerPhoneOutputDto
                {
                    Id = i,
                    PhoneNumber = $"55 31 9 {i.ToString().PadRight(4, '0')}-{customerId.ToString().PadLeft(4, '0')}"
                };
            }
        }
    }
}
