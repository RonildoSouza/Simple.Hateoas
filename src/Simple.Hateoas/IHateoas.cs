using Simple.Hateoas.Models;

namespace Simple.Hateoas
{
    public interface IHateoas
    {
        HateoasResult<TData> Create<TData>(TData data, params object[] args);
    }
}
