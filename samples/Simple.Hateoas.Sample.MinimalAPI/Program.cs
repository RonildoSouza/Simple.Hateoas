using Microsoft.AspNetCore.Mvc;
using Simple.Hateoas;
using Simple.Hateoas.Models;
using Simple.Hateoas.Sample.Core.Dtos;
using Simple.Hateoas.Sample.Core.HateoasLinkBuilders;
using Simple.Hateoas.Sample.Core.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSimpleHateoas(Assembly.GetAssembly(typeof(IPermissionServiceMock)));
builder.Services.AddScoped<IPermissionServiceMock, PermissionServiceMock>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var _customers = CustomerOutputDto.GetAll().ToList();
var customer = app.MapGroup("/customer")
    .WithTags("Customers");

customer.MapGet("/", (
    [FromServices] IHateoas hateoas,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10) =>
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

    return Results.Ok(hateoas.Create(pagedResult));
})
.WithName(CustomersRouterNames.GetCustomers)
.Produces<HateoasResult<PagedResult<CustomerOutputDto>>>(StatusCodes.Status200OK)
.WithOpenApi();

customer.MapGet("/{id}", (
    [FromServices] IHateoas hateoas,
    [FromRoute] int id) =>
{
    var customerOutputDto = _customers.FirstOrDefault(_ => _.Id == id);
    return customerOutputDto != null ? Results.Ok(hateoas.Create(customerOutputDto)) : Results.NotFound();
})
.WithName(CustomersRouterNames.GetCustomer)
.Produces<HateoasResult<CustomerOutputDto>>(StatusCodes.Status200OK)
.WithOpenApi();

customer.MapPost("/", (
    [FromServices] IHateoas hateoas,
    [FromBody] CustomerInputDto dto) =>
{
    var customerOutputDto = new CustomerOutputDto
    {
        Id = _customers.Count + 1,
        Name = dto.Name
    };

    _customers.Add(customerOutputDto);

    return Results.Created(string.Empty, hateoas.Create(customerOutputDto));
})
.WithName(CustomersRouterNames.CreateCustomer)
.Produces<HateoasResult<CustomerOutputDto>>(StatusCodes.Status201Created)
.WithOpenApi();

customer.MapPut("/{id}", (
    [FromServices] IHateoas hateoas,
    [FromRoute] int id,
    [FromBody] CustomerInputDto dto) =>
{
    var index = _customers.FindIndex(_ => _.Id == id);

    if (index == -1)
        return Results.NotFound();

    _customers[index].Name = dto.Name;

    return Results.Ok(hateoas.Create(_customers[index]));
})
.WithName(CustomersRouterNames.EditCustomer)
.Produces<HateoasResult<CustomerOutputDto>>(StatusCodes.Status200OK)
.WithOpenApi();

customer.MapDelete("/{id}", ([FromRoute] int id) =>
{
    _customers.RemoveAll(_ => _.Id == id);
    return Results.Ok();
})
.WithName(CustomersRouterNames.DeleteCustomer)
.Produces(StatusCodes.Status200OK)
.WithOpenApi();

customer.MapGet("{id}/phones", (
    [FromServices] IHateoas hateoas,
    [FromRoute] int id,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10) =>
{
    var customerPhones = CustomerPhoneOutputDto.GetAll(id).ToList();

    var pagedResult = new PagedResult<CustomerPhoneOutputDto>
    {
        CurrentPage = page,
        PageSize = pageSize,
    };

    pagedResult.RowCount = customerPhones.Count();

    var pageCount = (double)pagedResult.RowCount / pageSize;
    pagedResult.PageCount = (int)Math.Ceiling(pageCount);

    var skip = (page - 1) * pageSize;

    pagedResult.Results = customerPhones.Skip(skip).Take(pageSize);

    return Results.Ok(hateoas.Create(pagedResult, id));
})
.WithName(CustomersRouterNames.GetCustomerPhones)
.Produces<HateoasResult<PagedResult<CustomerPhoneOutputDto>>>(StatusCodes.Status200OK)
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
