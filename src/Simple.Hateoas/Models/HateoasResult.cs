using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Simple.Hateoas.Models
{
    public class HateoasResult<TData> : IDisposable
    {
        private readonly List<HateoasLink> _links = new List<HateoasLink>();
        private readonly IUrlHelper _urlHelper;

        public HateoasResult(IUrlHelper urlHelper, TData data)
        {
            _urlHelper = urlHelper;
            Data = data;
        }

        public TData Data { get; }
        public IReadOnlyList<HateoasLink> Links => _links;

        public HateoasResult<TData> AddLink(Func<TData, object> routeDataFunction, string routeName, string rel, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
        {
            _links.Add(new HateoasLink(_urlHelper.Link(routeName, routeDataFunction?.Invoke(Data)), rel, httpMethod.ToString().ToUpper()));

            if (whenPredicate != null)
                _links.RemoveAll(_ => _.Rel == rel && !whenPredicate.Invoke(Data));

            return this;
        }

        public HateoasResult<TData> AddLink(Func<TData, object> routeDataFunction, string routeName, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
            => AddLink(routeDataFunction, routeName, routeName, httpMethod, whenPredicate);

        public HateoasResult<TData> AddLink(string routeName, string rel, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
            => AddLink(null, routeName, rel, httpMethod, whenPredicate);

        public HateoasResult<TData> AddLink(string routeName, HttpMethod httpMethod, Func<TData, bool> whenPredicate = null)
            => AddLink(null, routeName, routeName, httpMethod, whenPredicate);

        public HateoasResult<TData> AddSelfLink(Func<TData, object> routeDataFunction, string routeName, Func<TData, bool> whenPredicate = null)
            => AddLink(routeDataFunction, routeName, "self", HttpMethod.Get, whenPredicate);

        public HateoasResult<TData> AddNextLink(Func<TData, object> routeDataFunction, string routeName, Func<TData, bool> whenPredicate = null)
            => AddLink(routeDataFunction, routeName, "next", HttpMethod.Get, whenPredicate);

        public HateoasResult<TData> AddPrevLink(Func<TData, object> routeDataFunction, string routeName, Func<TData, bool> whenPredicate = null)
            => AddLink(routeDataFunction, routeName, "prev", HttpMethod.Get, whenPredicate);

        public void Dispose()
        {
            _links.Clear();
        }
    }
}
