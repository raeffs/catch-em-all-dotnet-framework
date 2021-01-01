using System;
using Cirrus.Attributes;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_SearchResult), nameof(DataObjects.CatchEmAll_SearchResults))]
    internal class SearchResultSummary : IHasIndex
    {
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Hidden]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Marker))]
        [Icon]
        public string Marker { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        public string Name { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_BidPrice))]
        [Number(2)]
        public decimal? BidPrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_PurchasePrice))]
        [Number(2)]
        public decimal? PurchasePrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Ends))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? Ends { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_LastUpdated))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? LastUpdated { get; set; }
    }
}
