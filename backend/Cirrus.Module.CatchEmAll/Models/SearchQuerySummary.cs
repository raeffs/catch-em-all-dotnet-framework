using System;
using Cirrus.Attributes;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_SearchQuery), nameof(DataObjects.CatchEmAll_SearchQueries))]
    internal class SearchQuerySummary : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Number]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        public string Name { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Category))]
        [Text(TextType.SingleLine)]
        public string Category { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_AverageFinalPrice))]
        [Number(2)]
        public decimal? AverageFinalPrice { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_NumberOfResults))]
        [Number]
        public int NumberOfResults { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_NumberOfOpenResults))]
        [Number]
        public int NumberOfOpenResults { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_LastUpdated))]
        [DateTime(DateTimeType.DateTime)]
        public DateTime? LastUpdated { get; set; }

        [Choice(ChoiceType.Toggle)]
        [Name(nameof(DataObjects.CatchEmAll_NotificationsEnabled))]
        public bool EnableNotifications { get; set; }

        [Tags]
        [Name(nameof(DataObjects.CatchEmAll_Tags))]
        public string Tags { get; set; }
    }
}
