using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Simple.Hateoas.Test")]

namespace Simple.Hateoas.Internal
{
    internal class HateoasBuilderContext : IHateoasBuilderContext
    {
        private readonly IDictionary<Type, Type> _hateoasLinkBuilder = new Dictionary<Type, Type>();
        private IServiceProvider _serviceProvider;

        internal HateoasBuilderContext(Assembly assembly) => Initialize(assembly);

        public IHateoasLinkBuilder<TData> GetHateoasLinkBuilderInstance<TData>(Type key)
        {
            if (!_hateoasLinkBuilder.TryGetValue(key, out Type hateoasLinkBuilderType))
                throw new Exception($"{key.Name} not found!");

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

        public void SetServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        /// <summary>
        /// Find all classes classes that implement <see cref="IHateoasLinkBuilder{TData}"/>
        /// </summary>
        /// <param name="assembly">Assembly with classes that implement <see cref="IHateoasLinkBuilder{TData}"/></param>
        private void Initialize(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var hateoasLinkBuilders = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => IsHateoasLinkBuilder(i)));

            foreach (var builder in hateoasLinkBuilders)
            {
                var interfaceType = builder.GetInterfaces().Single(i => IsHateoasLinkBuilder(i));
                _hateoasLinkBuilder.TryAdd(interfaceType, builder);
            }

            static bool IsHateoasLinkBuilder(Type type)
                => type.IsInterface && type.Name.Contains(typeof(IHateoasLinkBuilder<>).Name);
        }
    }
}
