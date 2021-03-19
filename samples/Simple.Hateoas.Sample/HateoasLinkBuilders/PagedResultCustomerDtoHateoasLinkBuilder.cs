using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Dtos;

namespace Simple.Hateoas.Sample.HateoasLinkBuilders
{
    public class PagedResultCustomerDtoHateoasLinkBuilder : IHateoasLinkBuilder<PagedResult<CustomerOutputDto>>
    {
        public HateoasResult<PagedResult<CustomerOutputDto>> Build(HateoasResult<PagedResult<CustomerOutputDto>> hateoasResult)
        {
            hateoasResult
               .AddLink(CustomersRouterNames.CreateCustomer, HttpMethod.Post)
               .AddNextLink(p => new { page = p.CurrentPage + 1, pageSize = p.PageSize }, CustomersRouterNames.GetCustomers, _ => _.CurrentPage < _.PageCount)
               .AddPrevLink(p => new { page = p.CurrentPage - 1, pageSize = p.PageSize }, CustomersRouterNames.GetCustomers, _ => _.CurrentPage > 1);

            return hateoasResult;
        }
    }
}
