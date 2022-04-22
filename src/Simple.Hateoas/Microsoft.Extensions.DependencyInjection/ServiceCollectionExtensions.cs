using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Simple.Hateoas;
using Simple.Hateoas.Internal;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for simple hateoas
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="assembly">Assembly with classes that implement <see cref="IHateoasLinkBuilder{TData}"/></param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained</returns>
        public static IServiceCollection AddSimpleHateoas(this IServiceCollection services, Assembly assembly)
        {
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>()
                .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

            services.AddScoped(typeof(IHateoas), typeof(Hateoas));

            services.AddSingleton<IHateoasBuilderContext>(new HateoasBuilderContext(assembly));

            return services;
        }

        /// <summary>
        /// Adds services required for simple hateoas
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained</returns>
        public static IServiceCollection AddSimpleHateoas(this IServiceCollection services)
            => AddSimpleHateoas(services, Assembly.GetCallingAssembly());
    }
}
