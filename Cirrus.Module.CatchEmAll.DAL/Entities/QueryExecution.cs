using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("QueryExecutions", Schema = "catchemall")]
    public class QueryExecution : IHasIndex, IForTenant
    {
        [Index("IX_TenantIdStartSuccessful", 1, IsClustered = false, IsUnique = false)]
        public long TenantId { get; set; }

        public long Id { get; set; }

        [Index("IX_TenantIdStartSuccessful", 2, IsClustered = false, IsUnique = false)]
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        [Index("IX_QueryIdSuccessful", 2, IsClustered = false, IsUnique = false)]
        [Index("IX_TenantIdStartSuccessful", 3, IsClustered = false, IsUnique = false)]
        public bool Successful { get; set; }

        public bool IsUserInitiated { get; set; }

        [Index]
        [Index("IX_QueryIdSuccessful", 1, IsClustered = false, IsUnique = false)]
        public long QueryId { get; set; }

        [ForeignKey(nameof(QueryId))]
        public virtual SearchQuery Query { get; set; }

        public string Message { get; set; }
    }
}
