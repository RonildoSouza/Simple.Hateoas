using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Dtos;
using Simple.Hateoas.Sample.Services;
using System;

namespace Simple.Hateoas.Sample.HateoasLinkBuilders
{
    public class CustomerOutputDtoHateoasLinkBuilder : IHateoasLinkBuilder<CustomerOutputDto>
    {
        private readonly int _loggedUserPermissionId = new Random().Next(2);
        private readonly IPermissionServiceMock _permissionServiceMock;

        public CustomerOutputDtoHateoasLinkBuilder(IPermissionServiceMock permissionServiceMock)
        {
            _permissionServiceMock = permissionServiceMock;
        }

        public HateoasResult<CustomerOutputDto> Build(HateoasResult<CustomerOutputDto> hateoasResult)
        {
            hateoasResult
               .AddSelfLink(CustomersRouterNames.GetCustomer, c => new { id = c.Id })
               .AddLink(CustomersRouterNames.GetCustomerPhones, HttpMethod.Get, c => new { id = c.Id })
               .AddLink(CustomersRouterNames.EditCustomer, HttpMethod.Put, c => new { id = c.Id })
               .AddLink(CustomersRouterNames.DeleteCustomer, HttpMethod.Delete, c => new { id = c.Id }, _ => _permissionServiceMock.HasPermission(_loggedUserPermissionId));

            return hateoasResult;
        }
    }
}
