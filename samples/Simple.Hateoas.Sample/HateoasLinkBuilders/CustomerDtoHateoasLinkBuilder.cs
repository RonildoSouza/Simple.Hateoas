using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Dtos;
using Simple.Hateoas.Sample.Services;
using System;

namespace Simple.Hateoas.Sample.HateoasLinkBuilders
{
    public class CustomerDtoHateoasLinkBuilder : IHateoasLinkBuilder<CustomerOutputDto>
    {
        private readonly int _loggedUserPermissionId = new Random().Next(2);
        private readonly IPermissionServiceMock _permissionServiceMock;

        public CustomerDtoHateoasLinkBuilder(IPermissionServiceMock permissionServiceMock)
        {
            _permissionServiceMock = permissionServiceMock;
        }

        public HateoasResult<CustomerOutputDto> Build(HateoasResult<CustomerOutputDto> hateoasResult)
        {
            hateoasResult
               .AddSelfLink(c => new { id = c.Id }, CustomersRouterNames.GetCustomer)
               .AddLink(c => new { id = c.Id }, CustomersRouterNames.EditCustomer, HttpMethod.Put)
               .AddLink(c => new { id = c.Id }, CustomersRouterNames.DeleteCustomer, HttpMethod.Delete, _ => _permissionServiceMock.HasPermission(_loggedUserPermissionId));

            return hateoasResult;
        }
    }
}
