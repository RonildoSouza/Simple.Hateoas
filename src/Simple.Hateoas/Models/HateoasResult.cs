using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Hateoas.Models
{
    public class HateoasResult<TData> : IDisposable
    {
        private readonly List<HateoasLink> _links = new List<HateoasLink>();
        private readonly IUrlHelper _urlHelper;
        private readonly object[] _args;

        public HateoasResult(IUrlHelper urlHelper, TData data, params object[] args)
        {
            _urlHelper = urlHelper;
            Data = data;
            _args = args;
        }

        public TData Data { get; }
        public IReadOnlyList<HateoasLink> Links => _links;

        public HateoasResult<TData> AddLink(string routeName, string rel, HttpMethod httpMethod, Func<TData, object> routeDataFunction, Func<TData, bool> whenPredicate = null)
        {
            _links.Add(new HateoasLink(_urlHelper.Link(routeName, routeDataFunction?.Invoke(Data)), rel, httpMethod.ToString().ToUpper()));

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
