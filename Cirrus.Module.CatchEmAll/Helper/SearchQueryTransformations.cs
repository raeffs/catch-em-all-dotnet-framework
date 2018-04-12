using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class SearchQueryTransformations
    {
        public static readonly Func<DAL.Entities.SearchQuery, string> EntityToUrl = e =>
        {
            var values = new Dictionary<string, string>();
            values.Add(UseDescriptionKey, e.UseDescription ? bool.TrueString : bool.FalseString);
            values.Add(WithAllTheseWordsKey, e.WithAllTheseWords);
            values.Add(WithOneOfTheseWordsKey, e.WithOneOfTheseWords);
            values.Add(WithExactlyTheseWordsKey, e.WithExactlyTheseWords);
            values.Add(WithNoneOfTheseWordsKey, e.WithNoneOfTheseWords);
            values.Add(CategoryKey, e.Category.Number.ToString());
            values.Add(ConditionKey, ((int)e.Condition).ToString());

            var parts = values
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}");

            var queryString = string.Join("&", parts);

            return string.Format(UrlPattern, queryString);
        };

        private static readonly string UrlPattern = "https://www.ricardo.ch/search/index/?SortingType=1&PageSize=120&{0}";

        private static readonly string UseDescriptionKey = "UseDescription";
        private static readonly string WithAllTheseWordsKey = "SearchSentence";
        private static readonly string WithOneOfTheseWordsKey = "SearchOneOf";
        private static readonly string WithExactlyTheseWordsKey = "SearchFullMatch";
        private static readonly string WithNoneOfTheseWordsKey = "SearchExclude";
        private static readonly string CategoryKey = "CategoryNr";
        private static readonly string ConditionKey = "ItemCondition";
    }
}
