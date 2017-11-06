using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("ResultExecutions", Schema = "catchemall")]
    public class ResultExecution : IHasIndex, IForTenant
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        [Index("IX_ResultIdSuccessful", 2, IsClustered = false, IsUnique = false)]
        public bool Successful { get; set; }

        public bool IsUserInitiated { get; set; }

        [Index]
        [Index("IX_ResultIdSuccessful", 1, IsClustered = false, IsUnique = false)]
        public long ResultId { get; set; }

        [ForeignKey(nameof(ResultId))]
        public virtual SearchResult Result { get; set; }

        public string Message { get; set; }
    }
}
