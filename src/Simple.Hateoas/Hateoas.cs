using Microsoft.AspNetCore.Mvc;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;
using System.Linq;

namespace Simple.Hateoas
{
    public sealed class Hateoas : IHateoas
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IHateoasBuilderContext _hateoasBuilderContext;
        private readonly IServiceProvider _serviceProvider;

        public Hateoas(IUrlHelper urlHelper, IHateoasBuilderContext hateoasBuilderContext, IServiceProvider serviceProvider)
        {
            _urlHelper = urlHelper;
            _hateoasBuilderContext = hateoasBuilderContext;
            _serviceProvider = serviceProvider;
        }

        public HateoasResult<TData> Create<TData>(TData data, params object[] args)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var hateoasLinkBuilderType = _hateoasBuilderContext.GetHateoasLinkBuilderType(typeof(IHateoasLinkBuilder<TData>));
            var hateoasLinkBuilder = CreateInstanceHateoasLinkBuilder<TData>(hateoasLinkBuilderType);
            var hateoasResult = Activator.CreateInstance(typeof(HateoasResult<TData>), _urlHelper, data, args) as HateoasResult<TData>;

            return hateoasLinkBuilder.Build(hateoasResult);
        }

        private IHateoasLinkBuilder<TData> CreateInstanceHateoasLinkBuilder<TData>(Type hateoasLinkBuilderType)
        {
            var constructors = hateoasLinkBuilderType.GetConstructors();

            if (constructors?.Length > 1)
                throw new NotSupportedException($"{hateoasLinkBuilderType.Name} has more than 1 constructor!");

            var parameters = constructors.Single().GetParameters();

            if ((constructors?.Any() ?? false) && (parameters?.Any() ?? false))
            {
                var objectsToInject = parameters.Select(_ => _serviceProvider.GetService(_.ParameterType)).ToArray();
                return Activator.CreateInstance(hateoasLinkBuilderType, objectsToInject) as IHateoasLinkBuilder<TData>;
            }

            return Activator.CreateInstance(hateoasLinkBuilderType) as IHateoasLinkBuilder<TData>;
        }
    }
}
