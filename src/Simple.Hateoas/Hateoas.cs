using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;
using System.Reflection;

namespace Simple.Hateoas
{
    public sealed class Hateoas : IHateoas
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHateoasBuilderContext _hateoasBuilderContext;

        public Hateoas(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, IHateoasBuilderContext hateoasBuilderContext, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _hateoasBuilderContext = hateoasBuilderContext;
            _hateoasBuilderContext.SetServiceProvider(serviceProvider);
        }

        public HateoasResult<TData> Create<TData>(TData data, params object[] args)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var hateoasLinkBuilder = _hateoasBuilderContext.GetHateoasLinkBuilderInstance<TData>(typeof(IHateoasLinkBuilder<TData>));
            var hateoasResult = Activator.CreateInstance(
                type: typeof(HateoasResult<TData>),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { _httpContextAccessor, _linkGenerator, data, args },
                culture: null) as HateoasResult<TData>;

            return hateoasLinkBuilder.AddLinks(hateoasResult);
        }
    }
}
