using System;

namespace Simple.Hateoas.Internal
{
    public interface IHateoasBuilderContext
    {
        Type GetHateoasLinkBuilderType(Type key);
    }
}
