using System;
using Cirrus.Module.CatchEmAll.Helper;
using HtmlAgilityPack;

namespace Cirrus.Module.CatchEmAll.Models
{
    [System.Diagnostics.DebuggerDisplay("{ExternalId}: {Name}")]
    internal class SearchResultItem
    {
        private static readonly string ExternalIdAttribute = "data-article-number";
        private static readonly string NameQuery = ".//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-offer-title ')]";
        private static readonly string DescriptionQuery = ".//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-offer-subtitle ')]";
        private static readonly string EndsQuery = ".//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-enddate ')]";
        private static readonly string BidPriceQuery = ".//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-price-bid ')]/text()";
        private static readonly string PurchasePriceQuery = ".//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-price-bin ')]/text()";

        private readonly HtmlNode rootNode;

        public SearchResultItem(HtmlNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public long ExternalId
        {
            get
            {
                var attribute = this.rootNode.Attributes[ExternalIdAttribute];
                var value = Parse.ToLong(attribute?.Value);
                Ensure.NotNull(value, nameof(this.ExternalId));
                return value.Value;
            }
        }

        public string Name
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(NameQuery);
                var value = Parse.ToString(node?.InnerText);
                Ensure.NotNull(value, nameof(this.Name));
                return value;
            }
        }

        public string Description
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(DescriptionQuery);
                var value = Parse.ToString(node?.InnerText);
                return value;
            }
        }

        public DateTime? Ends
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(EndsQuery);
                var value = Parse.ToDateTime(node?.InnerText);
                return value;
            }
        }

        public decimal? BidPrice
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(BidPriceQuery);
                var value = Parse.ToDecimal(node?.InnerText);
                return value;
            }
        }

        public decimal? PurchasePrice
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(PurchasePriceQuery);
                var value = Parse.ToDecimal(node?.InnerText);
                return value;
            }
        }
    }
}
