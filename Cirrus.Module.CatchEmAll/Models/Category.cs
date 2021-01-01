using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Category), nameof(DataObjects.CatchEmAll_Categories))]
    internal class Category : IHasIndex
    {
        [Hidden]
        [Name(nameof(DataObjects.CatchEmAll_Id))]
        public long Id { get; set; }

        [DisplayOrder(1)]
        [Required]
        [Name(nameof(DataObjects.CatchEmAll_Number))]
        [Number]
        public int Number { get; set; }

        [DisplayOrder(2)]
        [Required]
        [PrimaryField]
        [Name(nameof(DataObjects.CatchEmAll_Name))]
        [Text(TextType.SingleLine)]
        public string Name { get; set; }
    }
}
