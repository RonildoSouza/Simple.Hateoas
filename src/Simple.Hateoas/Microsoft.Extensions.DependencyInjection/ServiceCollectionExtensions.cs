using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Simple.Hateoas;
using Simple.Hateoas.Internal;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleHateoas(this IServiceCollection services, Assembly assembly)
        {
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>()
                .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

            services.AddSingleton<IHateoasBuilderContext>(new HateoasBuilderContext(assembly));

            services.AddScoped(typeof(IHateoas), typeof(Hateoas));

            return services;
        }

        public static IServiceCollection AddSimpleHateoas(this IServiceCollection services)
            => AddSimpleHateoas(services, Assembly.GetCallingAssembly());
    }
}
