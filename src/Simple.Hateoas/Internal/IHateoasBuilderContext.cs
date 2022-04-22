using System;

namespace Simple.Hateoas.Internal
{
    public interface IHateoasBuilderContext
    {
        IHateoasLinkBuilder<TData> GetHateoasLinkBuilderInstance<TData>(Type hateoasLinkBuilderType);
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}
