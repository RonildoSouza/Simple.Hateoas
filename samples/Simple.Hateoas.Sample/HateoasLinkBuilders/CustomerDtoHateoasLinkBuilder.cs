using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Dtos;

namespace Simple.Hateoas.Sample.HateoasLinkBuilders
{
    public class CustomerDtoHateoasLinkBuilder : IHateoasLinkBuilder<CustomerOutputDto>
    {
        public HateoasResult<CustomerOutputDto> Build(HateoasResult<CustomerOutputDto> hateoasResult)
        {
            hateoasResult
               .AddSelfLink(c => new { id = c.Id }, CustomersRouterNames.GetCustomer)
               .AddLink(c => new { id = c.Id }, CustomersRouterNames.EditCustomer, HttpMethod.Put);

            return hateoasResult;
        }
    }
}
