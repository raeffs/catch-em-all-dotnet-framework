using System;
using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Feedback), nameof(DataObjects.CatchEmAll_Feedbacks))]
    internal class FeedbackSummary : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        public long Id { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Name))]
        public string Name { get; set; }

        [Name(nameof(DataObjects.CatchEmAll_Username))]
        public string Username { get; set; }

        [DropDown]
        [Name(nameof(DataObjects.CatchEmAll_Status))]
        public FeedbackStatus Status { get; set; }

        [DateTime(DateTimeType.DateTime)]
        [Name(nameof(DataObjects.CatchEmAll_Created))]
        public DateTime Created { get; set; }
    }
}
