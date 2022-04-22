using Simple.Hateoas.Models;

namespace Simple.Hateoas
{
    public interface IHateoasLinkBuilder<TData>
    {
        /// <summary>
        /// Adds links on hateoas result before return <see cref="IHateoas.Create{TData}(TData, object[])"/>
        /// </summary>
        HateoasResult<TData> AddLinks(HateoasResult<TData> hateoasResult);
    }
}
