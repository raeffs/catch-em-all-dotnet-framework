using System.Collections.Generic;
using Cirrus.Attributes;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_SearchQuery), nameof(DataObjects.CatchEmAll_SearchQueries))]
    internal class SearchQuery : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Number]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        [Required]
        [PrimaryField]
        public string Name { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_WithAllTheseWords))]
        [Text(TextType.SingleLine)]
        public string WithAllTheseWords { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_WithOneOfTheseWords))]
        [Text(TextType.SingleLine)]
        public string WithOneOfTheseWords { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_WithExactlyTheseWords))]
        [Text(TextType.SingleLine)]
        public string WithExactlyTheseWords { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_WithNoneOfTheseWords))]
        [Text(TextType.SingleLine)]
        public string WithNoneOfTheseWords { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Category))]
        [DropDown]
        [Required]
        public DropDown CategorySelection { get; set; }

        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_UseDescription))]
        [Choice(ChoiceType.Toggle)]
        public bool UseDescription { get; set; }

        [CountOf(nameof(Executions))]
        [Name(nameof(DataObjects.CatchEmAll_ExecutionCount))]
        public long ExecutionCount { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Executions))]
        public ICollection<ExecutionSummary> Executions { get; set; } = new List<ExecutionSummary>();

        [Choice(ChoiceType.Toggle)]
        [Group(nameof(DataObjects.CatchEmAll_Miscellaneous))]
        [Name(nameof(DataObjects.CatchEmAll_AutoFilterDeletedDuplicates))]
        public bool AutoFilterDeletedDuplicates { get; set; }

        [Tags(AllowCustomInputs = true)]
        [Group(nameof(DataObjects.CatchEmAll_Miscellaneous))]
        [Name(nameof(DataObjects.CatchEmAll_Tags))]
        public Tag TagSelection { get; set; }

        [Choice(ChoiceType.Toggle)]
        [Group(nameof(DataObjects.CatchEmAll_Miscellaneous))]
        [Name(nameof(DataObjects.CatchEmAll_EnableNotifications))]
        public bool EnableNotifications { get; set; }

        [DropDown]
        [Group(nameof(DataObjects.CatchEmAll_Miscellaneous))]
        [Name(nameof(DataObjects.CatchEmAll_NotificationMode))]
        public DAL.Entities.NotificationMode NotificationMode { get; set; }

        [Number(2)]
        [Group(nameof(DataObjects.CatchEmAll_Miscellaneous))]
        [Name(nameof(DataObjects.CatchEmAll_DesiredPrice))]
        public decimal? DesiredPrice { get; set; }

        [Tags(AllowCustomInputs = false)]
        [Name(nameof(DataObjects.CatchEmAll_Condition))]
        public Tag Condition { get; set; }
    }
}
