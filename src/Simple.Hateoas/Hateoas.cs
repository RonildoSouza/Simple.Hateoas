using Microsoft.AspNetCore.Mvc;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;

namespace Simple.Hateoas
{
    public sealed class Hateoas : IHateoas
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IHateoasBuilderContext _hateoasBuilderContext;

        public Hateoas(IUrlHelper urlHelper, IHateoasBuilderContext hateoasBuilderContext)
        {
            _urlHelper = urlHelper;
            _hateoasBuilderContext = hateoasBuilderContext;
        }

        public HateoasResult<TData> Create<TData>(TData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var hateoasLinkBuilderType = _hateoasBuilderContext.GetHateoasLinkBuilderType(typeof(IHateoasLinkBuilder<TData>));
            var hateoasLinkBuilder = Activator.CreateInstance(hateoasLinkBuilderType) as IHateoasLinkBuilder<TData>;
            var hateoasResult = Activator.CreateInstance(typeof(HateoasResult<TData>), _urlHelper, data) as HateoasResult<TData>;

            return hateoasLinkBuilder.Build(hateoasResult);
        }
    }
}
