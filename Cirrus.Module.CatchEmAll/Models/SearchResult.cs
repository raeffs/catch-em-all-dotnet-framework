using System;
using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_SearchResult), nameof(DataObjects.CatchEmAll_SearchResults))]
    internal class SearchResult : IHasIndex
    {
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Hidden]
        public long Id { get; set; }

        [Readonly]
        [PrimaryField]
        [DisplayOrder(1)]
        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        public string Name { get; set; }

        [Readonly]
        [DisplayOrder(2)]
        [Name(nameof(DataObjects.CatchEmAll_Description))]
        [Text(TextType.SingleLine)]
        public string Description { get; set; }

        [Readonly]
        [Name(nameof(DataObjects.CatchEmAll_Ends))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? Ends { get; set; }

        [Readonly]
        [Name(nameof(DataObjects.CatchEmAll_BidPrice))]
        [Number(2)]
        public decimal? BidPrice { get; set; }

        [Readonly]
        [Name(nameof(DataObjects.CatchEmAll_PurchasePrice))]
        [Number(2)]
        public decimal? PurchasePrice { get; set; }

        [Readonly]
        [Name(nameof(DataObjects.CatchEmAll_FinalPrice))]
        [Number(2)]
        public decimal? FinalPrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_ExternalData))]
        public ExternalSearchResultData ExternalData { get; set; }

        public bool Closed { get; set; }

        public bool Sold { get; set; }

        public bool Favorite { get; set; }
    }
}
