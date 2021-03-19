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

        internal HateoasBuilderContext(Assembly assembly)
        {
            Initialize(assembly);
        }

        public Type GetHateoasLinkBuilderType(Type key)
        {
            if (!_hateoasLinkBuilder.TryGetValue(key, out Type value))
                throw new Exception($"{key.Name} not found!");

            return value;
        }

        private void Initialize(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var hateoasLinkBuilders = assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => IsHateoasLinkBuilder(i)))
                .ToList();

            foreach (var builder in hateoasLinkBuilders)
            {
                var interfaceType = builder.GetInterfaces().Single(i => IsHateoasLinkBuilder(i));

                if (!_hateoasLinkBuilder.ContainsKey(interfaceType))
                    _hateoasLinkBuilder.Add(interfaceType, builder);
            }
        }

        private bool IsHateoasLinkBuilder(Type type)
        {
            return type.IsInterface && type.Name.Contains(typeof(IHateoasLinkBuilder<>).Name);
        }
    }
}
