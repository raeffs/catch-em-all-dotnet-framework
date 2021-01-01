using System;
using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Execution), nameof(DataObjects.CatchEmAll_Executions))]
    internal class QueryExecutionSummary : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        public long Id { get; set; }

        [DateTime(DateTimeType.DateTime)]
        [Name(nameof(DataObjects.CatchEmAll_Start))]
        public DateTime Start { get; set; }

        [DateTime(DateTimeType.DateTime)]
        [Name(nameof(DataObjects.CatchEmAll_End))]
        public DateTime? End { get; set; }

        [Choice(ChoiceType.Checkbox)]
        [Name(nameof(DataObjects.CatchEmAll_Successful))]
        public bool Successful { get; set; }

        [Choice(ChoiceType.Checkbox)]
        [Name(nameof(DataObjects.CatchEmAll_IsUserInitiated))]
        public bool IsUserInitiated { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_SearchQuery))]
        public string Query { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Category))]
        public string Category { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Username))]
        public string Username { get; set; }
    }
}
