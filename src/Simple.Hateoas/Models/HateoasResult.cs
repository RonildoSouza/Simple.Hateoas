using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Hateoas.Models
{
    /// <summary>
    /// Simple hateoas result structure
    /// </summary>
    /// <typeparam name="TData">Type from model result</typeparam>
    public class HateoasResult<TData> : IDisposable
    {
        private readonly List<HateoasLink> _links = new List<HateoasLink>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly object[] _args;

        internal HateoasResult(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, TData data, params object[] args)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            Data = data;
            _args = args;
        }

        public TData Data { get; }
        public IReadOnlyList<HateoasLink> Links => _links;

        public HateoasResult<TData> AddLink(string routeName, string rel, HttpMethod httpMethod, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
        {
            _links.Add(new HateoasLink(
                href: _linkGenerator.GetUriByName(_httpContextAccessor.HttpContext, routeName, routeDataFunction?.Invoke(Data)),
                rel: rel,
                method: httpMethod.ToString().ToUpper()));

            if (whenPredicate != null)
                _links.RemoveAll(_ => _.Rel == rel && !whenPredicate.Invoke(Data));

            return this;
        }

        public HateoasResult<TData> AddLink(string routeName, HttpMethod httpMethod, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, routeName, httpMethod, routeDataFunction, whenPredicate);

        public HateoasResult<TData> AddLink(string routeName, string rel, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, rel, httpMethod, null, whenPredicate);

        public HateoasResult<TData> AddLink(string routeName, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, routeName, httpMethod, null, whenPredicate);

        public HateoasResult<TData> AddSelfLink(string routeName, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, "self", HttpMethod.Get, routeDataFunction, whenPredicate);

        public HateoasResult<TData> AddNextLink(string routeName, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, "next", HttpMethod.Get, routeDataFunction, whenPredicate);

        public HateoasResult<TData> AddPrevLink(string routeName, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
            => AddLink(routeName, "prev", HttpMethod.Get, routeDataFunction, whenPredicate);

        public int GetTotalArgs() => _args?.Length ?? 0;

        public object GetArg(int index)
        {
            if (!(_args?.Any() ?? false))
                return null;

            return _args.ElementAt(index);
        }

        public void Dispose()
        {
            _links.Clear();
        }
    }
}
