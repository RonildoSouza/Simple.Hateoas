using Simple.Hateoas.Models;

namespace Simple.Hateoas
{
    public interface IHateoas
    {
        /// <summary>
        /// Create hateoas result
        /// </summary>
        /// <typeparam name="TData">Type from model result</typeparam>
        /// <param name="data">Model result</param>
        /// <param name="args">Any args to build hateoas result links. <para>This args can be access using method <see cref="HateoasResult{TData}.GetArg(int)"/>.</para></param>
        /// <returns><see cref="HateoasResult{TData}"/></returns>
        HateoasResult<TData> Create<TData>(TData data, params object[] args);
    }
}
