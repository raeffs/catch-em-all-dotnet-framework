using System;
using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_ArchivedSearchResult), nameof(DataObjects.CatchEmAll_ArchivedSearchResults))]
    internal class ArchivedSearchResultSummary : IHasIndex
    {
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Hidden]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        public string Name { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Sold))]
        [Choice(ChoiceType.Toggle)]
        public bool Sold { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_BidPrice))]
        [Number(2)]
        public decimal? BidPrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_PurchasePrice))]
        [Number(2)]
        public decimal? PurchasePrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_FinalPrice))]
        [Number(2)]
        public decimal? FinalPrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Ended))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? Ended { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_LastUpdated))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? LastUpdated { get; set; }
    }
}
