using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("SavedFilters", Schema = "catchemall")]
    public class SavedFilter : IHasIndex, IForTenant, IForUser
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserReference User { get; set; }

        public string WorkflowStepId { get; set; }

        public string OrderByProperties { get; set; }

        public bool OrderByAscending { get; set; }
    }
}
