# <a href="https://www.nuget.org/packages/Simple.Hateoas">Simple.Hateoas <img width="100px" src="https://learn.microsoft.com/pt-br/dotnet/standard/library-guidance/media/nuget/nuget-logo.png"></a>
A simple implementation of HATEOAS for .NET Web API to apply semantic links in models returned from your API.

[![Build, Tests and Publish](https://github.com/RonildoSouza/Simple.Hateoas/actions/workflows/dotnet.yml/badge.svg)](https://github.com/RonildoSouza/Simple.Hateoas/actions/workflows/dotnet.yml)


## Using Instructions
### *See samples for more details about using: https://github.com/RonildoSouza/Simple.Hateoas/tree/master/samples*

### **1 - Install Simple.Hateoas**
```
dotnet add package Simple.Hateoas --version 2.0.0 
```

### **2 - Register Simple.Hateoas**
```csharp
public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services.AddSimpleHateoas();
    }
    // ...
}
```

### OR (Minimal API)
```csharp
/// Program.cs

// ...
builder.Services.AddSimpleHateoas();
// ...
```

### **3 - Create your Hateoas Link Builder class**
```csharp
using Simple.Hateoas.Models;
using YourProject.Dtos

namespace YourProject.HateoasLinkBuilders
{
    public class EntityDtoHateoasLinkBuilder : IHateoasLinkBuilder<EntityDto>
    {
        private readonly IPermissionServiceMock _permissionServiceMock;

        public EntityDtoHateoasLinkBuilder(IPermissionServiceMock permissionServiceMock)
        {
            _permissionServiceMock = permissionServiceMock;
        }

        public HateoasResult<EntityDto> AddLinks(HateoasResult<EntityDto> hateoasResult)
        {
            hateoasResult
               .AddSelfLink("GetEntity", c => new { id = c.Id })
               .AddLink("DeleteEntity", HttpMethod.Delete, c => new { id = c.Id }, _ => _permissionServiceMock.UserLoggedIsAdmin());

            return hateoasResult;
        }
    }
}
```

### **4 - Create and return your Hateoas Result**
```csharp
using Microsoft.AspNetCore.Mvc;
using YourProject.Dtos;
using YourProject.HateoasLinkBuilders;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IEntityAppServiceMock _entityAppServiceMock;
        private readonly IHateoas _hateoas;

        public EntitiesController(IHateoas hateoas, IEntityAppServiceMock entityAppServiceMock)
        {
            _hateoas = hateoas;
            _entityAppServiceMock = entityAppServiceMock;
        }

        [HttpGet("{id}", Name = "GetEntity")]
        [ProducesResponseType(typeof(HateoasResult<EntityDto>), (int)HttpStatusCode.OK)]
        public IActionResult Get(Guid id)
        {
            var entityDto = _entityAppServiceMock.GetById(id);
            var hateoasResult = _hateoas.Create(entityDto);

            return Ok(hateoasResult);
        }        

        [HttpDelete("{id}", Name = "DeleteEntity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Delete(Guid id)
        {
            _entityAppServiceMock.RemoveById(id);
            return Ok();
        }
    }
}
```

### OR (Minimal API)
```csharp
app.MapGet("/{id}", (
    [FromServices] IHateoas hateoas,
    [FromServices] IEntityAppServiceMock entityAppServiceMock,
    [FromRoute] Guid id) =>
{
    var entityDto = entityAppServiceMock.GetById(id);
    var hateoasResult = hateoas.Create(entityDto);

    return Results.Ok(hateoasResult);
})
.WithName("GetEntity")
.Produces<HateoasResult<EntityDto>>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapDelete("/{id}", (
    [FromServices] IEntityAppServiceMock entityAppServiceMock,
    [FromRoute] Guid id) =>
{
    entityAppServiceMock.RemoveById(id);
    return Results.Ok();
})
.WithName("DeleteEntity")
.Produces(StatusCodes.Status200OK)
.WithOpenApi();
```
