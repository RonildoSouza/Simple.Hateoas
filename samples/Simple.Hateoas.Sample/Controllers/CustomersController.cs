using Microsoft.AspNetCore.Mvc;
using Simple.Hateoas.Sample.Dtos;
using Simple.Hateoas.Sample.HateoasLinkBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Simple.Hateoas.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private static readonly List<CustomerOutputDto> _customers = CustomerOutputDto.GetAll().ToList();
        private readonly IHateoas _hateoas;

        public CustomersController(IHateoas hateoas)
        {
            _hateoas = hateoas;
        }

        [HttpGet(Name = CustomersRouterNames.GetCustomers)]
        [ProducesResponseType(typeof(PagedResult<CustomerOutputDto>), (int)HttpStatusCode.OK)]
        public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = new PagedResult<CustomerOutputDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
            };

            pagedResult.RowCount = _customers.Count();

            var pageCount = (double)pagedResult.RowCount / pageSize;
            pagedResult.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;

            pagedResult.Results = _customers.Skip(skip).Take(pageSize);

            return page <= pageCount ? Ok(_hateoas.Create(pagedResult)) : NotFound();
        }

        [HttpGet("{id}", Name = CustomersRouterNames.GetCustomer)]
        [ProducesResponseType(typeof(CustomerOutputDto), (int)HttpStatusCode.OK)]
        public IActionResult Get(int id)
        {
            var customerOutputDto = _customers.FirstOrDefault(_ => _.Id == id);
            return customerOutputDto != null ? Ok(_hateoas.Create(customerOutputDto)) : NotFound();
        }

        [HttpPost(Name = CustomersRouterNames.CreateCustomer)]
        [ProducesResponseType(typeof(CustomerOutputDto), (int)HttpStatusCode.Created)]
        public IActionResult Post(CustomerInputDto dto)
        {
            var customerOutputDto = new CustomerOutputDto
            {
                Id = _customers.Count + 1,
                Name = dto.Name
            };

            _customers.Add(customerOutputDto);

            return Created(string.Empty, _hateoas.Create(customerOutputDto));
        }

        [HttpPut("{id}", Name = CustomersRouterNames.EditCustomer)]
        [ProducesResponseType(typeof(CustomerOutputDto), (int)HttpStatusCode.Created)]
        public IActionResult Post(int id, CustomerInputDto dto)
        {
            var index = _customers.FindIndex(_ => _.Id == id);

            if (index == -1)
                return NotFound();

            _customers[index].Name = dto.Name;

            return Ok(_hateoas.Create(_customers[index]));
        }
    }
}
