using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Cirrus.Module.CatchEmAll.Models
{
    internal class SearchQueryPage
    {
        private static readonly string ResultsQuery = ".//ul[contains(concat(' ', normalize-space(@class), ' '), ' ric-normal-offers ')][1]/li[contains(concat(' ', normalize-space(@class), ' '), ' container-fluid ')]";

        private readonly HtmlNode rootNode;

        public SearchQueryPage(HtmlNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public IEnumerable<SearchResultItem> Results
        {
            get { return this.rootNode.SelectNodes(ResultsQuery)?.Select(n => new SearchResultItem(n)) ?? new SearchResultItem[] { }; }
        }
    }
}
