using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Schedule), nameof(DataObjects.CatchEmAll_Schedules))]
    internal class Schedule : IHasIndex
    {
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        [Hidden]
        [Number]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_ExecutionType))]
        [DropDown]
        [Required]
        public ExecutionType Type { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_CronExpression))]
        [Text(TextType.SingleLine)]
        [Required]
        public string CronExpression { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_UpdateIntervalInMinutes))]
        [Number(0, false)]
        [Range(1, 10080)]
        public long UpdateIntervalInMinutes { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_IsDefault))]
        [Choice(ChoiceType.Toggle)]
        public bool IsDefault { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_IsEnabled))]
        [Choice(ChoiceType.Toggle)]
        public bool IsEnabled { get; set; }
    }
}
