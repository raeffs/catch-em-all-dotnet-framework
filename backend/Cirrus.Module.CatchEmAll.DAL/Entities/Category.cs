using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("Categories", Schema = "catchemall")]
    public class Category : IHasIndex, IForTenant, IForUser
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public int Number { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<SearchQuery> Queries { get; set; } = new HashSet<SearchQuery>();

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserReference User { get; set; }
    }
}
