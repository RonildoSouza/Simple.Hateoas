using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Dtos;

namespace Simple.Hateoas.Sample.HateoasLinkBuilders
{
    public class PagedResultCustomerPhoneOutputDtoHateoasLinkBuilder : IHateoasLinkBuilder<PagedResult<CustomerPhoneOutputDto>>
    {
        public HateoasResult<PagedResult<CustomerPhoneOutputDto>> AddLinks(HateoasResult<PagedResult<CustomerPhoneOutputDto>> hateoasResult)
        {
            var customerId = hateoasResult.GetArg(0);

            hateoasResult
               .AddSelfLink(CustomersRouterNames.GetCustomerPhones, p => new { id = customerId, page = p.CurrentPage - 1, pageSize = p.PageSize })
               .AddNextLink(CustomersRouterNames.GetCustomerPhones, p => new { page = p.CurrentPage + 1, pageSize = p.PageSize }, _ => _.CurrentPage < _.PageCount)
               .AddPrevLink(CustomersRouterNames.GetCustomerPhones, p => new { page = p.CurrentPage - 1, pageSize = p.PageSize }, _ => _.CurrentPage > 1);

            return hateoasResult;
        }
    }
}
