using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Module.CatchEmAll.DAL.Entities;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Feedback), nameof(DataObjects.CatchEmAll_Feedbacks))]
    internal class Feedback : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        public long Id { get; set; }

        [Required]
        [PrimaryField]
        [Text(TextType.SingleLine)]
        [Name(nameof(DataObjects.CatchEmAll_Name))]
        public string Name { get; set; }

        [Text(TextType.MultiLine)]
        [Name(nameof(DataObjects.CatchEmAll_Description))]
        public string Description { get; set; }

        [Required]
        [DropDown]
        [Name(nameof(DataObjects.CatchEmAll_Status))]
        public FeedbackStatus Status { get; set; }

        [Hidden]
        public bool BelongsToCurrentUser { get; set; }
    }
}
