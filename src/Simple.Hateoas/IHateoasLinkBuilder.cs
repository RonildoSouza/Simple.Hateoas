using Simple.Hateoas.Models;

namespace Simple.Hateoas
{
    public interface IHateoasLinkBuilder<TData>
    {
        HateoasResult<TData> Build(HateoasResult<TData> hateoasResult);
    }
}
