﻿using System;
using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Execution), nameof(DataObjects.CatchEmAll_Executions))]
    internal class ExecutionSummary : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        public long Id { get; set; }

        [Readonly]
        [DateTime(DateTimeType.DateTime)]
        [Name(nameof(DataObjects.CatchEmAll_Start))]
        public DateTime Start { get; set; }

        [Readonly]
        [DateTime(DateTimeType.DateTime)]
        [Name(nameof(DataObjects.CatchEmAll_End))]
        public DateTime? End { get; set; }

        [Readonly]
        [Choice(ChoiceType.Checkbox)]
        [Name(nameof(DataObjects.CatchEmAll_Successful))]
        public bool Successful { get; set; }

        [Readonly]
        [Choice(ChoiceType.Checkbox)]
        [Name(nameof(DataObjects.CatchEmAll_IsUserInitiated))]
        public bool IsUserInitiated { get; set; }
    }
}