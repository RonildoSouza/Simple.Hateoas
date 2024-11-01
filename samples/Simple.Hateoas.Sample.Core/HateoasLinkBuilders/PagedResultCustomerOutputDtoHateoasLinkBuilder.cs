using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Core.Dtos;

namespace Simple.Hateoas.Sample.Core.HateoasLinkBuilders
{
    public class PagedResultCustomerOutputDtoHateoasLinkBuilder : IHateoasLinkBuilder<PagedResult<CustomerOutputDto>>
    {
        public HateoasResult<PagedResult<CustomerOutputDto>> AddLinks(HateoasResult<PagedResult<CustomerOutputDto>> hateoasResult)
        {
            hateoasResult
               .AddLink(CustomersRouterNames.CreateCustomer, HttpMethod.Post)
               .AddNextLink(CustomersRouterNames.GetCustomers, p => new { page = p.CurrentPage + 1, pageSize = p.PageSize }, _ => _.CurrentPage < _.PageCount)
               .AddPrevLink(CustomersRouterNames.GetCustomers, p => new { page = p.CurrentPage - 1, pageSize = p.PageSize }, _ => _.CurrentPage > 1);

            return hateoasResult;
        }
    }
}
