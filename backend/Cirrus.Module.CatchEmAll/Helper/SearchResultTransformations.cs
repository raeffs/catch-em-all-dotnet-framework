using System;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class SearchResultTransformations
    {
        public static readonly Func<DAL.Entities.SearchResult, string> EntityToUrl = e =>
        {
            return string.Format(UrlPattern, e.ExternalId);
        };

        public static readonly Func<Models.SearchResult, string> ModelToUrl = e =>
        {
            return string.Format(UrlPattern, e.ExternalData.ExternalId);
        };

        private static readonly string UrlPattern = "https://www.ricardo.ch/v/an{0}/";
    }
}
