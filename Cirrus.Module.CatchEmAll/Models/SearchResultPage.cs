using System;
using Cirrus.Module.CatchEmAll.Helper;
using HtmlAgilityPack;

namespace Cirrus.Module.CatchEmAll.Models
{
    internal class SearchResultPage
    {
        private static readonly string EndsQuery = ".//div[contains(concat(' ', normalize-space(@class), ' '), ' details-container ')]//section[contains(concat(' ', normalize-space(@class), ' '), ' pdp-buybox ')]//span[contains(concat(' ', normalize-space(@class), ' '), ' ric-date ')]/text()";
        private static readonly string SoldQuery = ".//div[contains(concat(' ', normalize-space(@class), ' '), ' details-container ')]//section[contains(concat(' ', normalize-space(@class), ' '), ' pdp-buybox ')]//div[@data-qa='closed-article-message']";
        private static readonly string FinalPriceQuery = ".//div[contains(concat(' ', normalize-space(@class), ' '), ' details-container ')]//section[contains(concat(' ', normalize-space(@class), ' '), ' pdp-buybox ')]//div[@data-qa='closed-article-message']//span/text()";
        private static readonly string BidPriceQuery = ".//div[contains(concat(' ', normalize-space(@class), ' '), ' details-container ')]//section[contains(concat(' ', normalize-space(@class), ' '), ' r-buybox ')]//div[contains(concat(' ', normalize-space(@class), ' '), ' r-auction ')]//span/text()";
        private static readonly string PurchasePriceQuery = ".//div[contains(concat(' ', normalize-space(@class), ' '), ' details-container ')]//section[contains(concat(' ', normalize-space(@class), ' '), ' r-buybox ')]//div[contains(concat(' ', normalize-space(@class), ' '), ' r-buynow ')]//span//span//span/text()";

        private readonly HtmlNode rootNode;

        public SearchResultPage(HtmlNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public DateTime? Ends
        {
            get
            {
                var node = this.rootNode.SelectSingleNode(EndsQuery);
                var value = Parse.ToDateTime(node?.InnerText);
                Ensure.NotNull(value, nameof(this.Ends));
                return value;
            }
        }

        public bool Closed
        {
            get
            {
                return this.Ends.Value < DateTime.Now;
            }
        }

        public bool Sold
        {
            get
            {
                if (!this.Closed)
                {
                    return false;
                }

                var node = this.rootNode.SelectSingleNode(SoldQuery);
                var value = Parse.ToString(node?.InnerText);
                Ensure.NotNull(value, nameof(this.Sold));
                return !value.Contains("leider nicht");
            }
        }

        public decimal? BidPrice
        {
            get
            {
                if (this.Closed)
                {
                    return null;
                }

                var node = this.rootNode.SelectSingleNode(BidPriceQuery);
                return Parse.ToDecimal(node?.InnerText);
            }
        }

        public decimal? PurchasePrice
        {
            get
            {
                if (this.Closed)
                {
                    return null;
                }

                var node = this.rootNode.SelectSingleNode(PurchasePriceQuery);
                return Parse.ToDecimal(node?.InnerText);
            }
        }

        public decimal? FinalPrice
        {
            get
            {
                if (!this.Sold)
                {
                    return null;
                }

                var node = this.rootNode.SelectSingleNode(FinalPriceQuery);
                return Parse.ToDecimal(node?.InnerText);
            }
        }
    }
}
