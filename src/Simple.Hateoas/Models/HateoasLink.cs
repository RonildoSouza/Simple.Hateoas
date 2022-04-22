namespace Simple.Hateoas.Models
{
    /// <summary>
    /// Simple hateoas result links
    /// </summary>
    public sealed class HateoasLink
    {
        internal HateoasLink(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        public string Href { get; }
        public string Rel { get; }
        public string Method { get; }
    }
}
