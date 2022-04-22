using Microsoft.AspNetCore.Mvc;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;
using System.Reflection;

namespace Simple.Hateoas
{
    public sealed class Hateoas : IHateoas
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IHateoasBuilderContext _hateoasBuilderContext;

        public Hateoas(IUrlHelper urlHelper, IHateoasBuilderContext hateoasBuilderContext, IServiceProvider serviceProvider)
        {
            _urlHelper = urlHelper;
            _hateoasBuilderContext = hateoasBuilderContext;
            _hateoasBuilderContext.SetServiceProvider(serviceProvider);
        }

        public HateoasResult<TData> Create<TData>(TData data, params object[] args)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var hateoasLinkBuilder = _hateoasBuilderContext.GetHateoasLinkBuilderInstance<TData>(typeof(IHateoasLinkBuilder<TData>));
            var hateoasResult = Activator.CreateInstance(
                typeof(HateoasResult<TData>),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] { _urlHelper, data, args },
                null) as HateoasResult<TData>;

            return hateoasLinkBuilder.AddLinks(hateoasResult);
        }
    }
}
