namespace Simple.Hateoas.Models
{
    public sealed class HateoasLink
    {
        public HateoasLink(string href, string rel, string method)
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
