using Cirrus.Attributes;
using Cirrus.Module.CatchEmAll.DAL;

namespace Cirrus.Module.CatchEmAll.Models
{
    internal class CategoryWithQueryCount : Category
    {
        [Name(nameof(DataObjects.CatchEmAll_NumberOfQueries))]
        [Number]
        public int NumberOfQueries { get; set; }
    }
}
