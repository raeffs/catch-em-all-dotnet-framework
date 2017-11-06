using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("SearchResults", Schema = "catchemall")]
    public class SearchResult : IHasIndex, IForTenant
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        [Index("IX_ExternalId_QueryId", 1, IsUnique = true)]
        public long ExternalId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Index("IX_ExternalId_QueryId", 2, IsUnique = true)]
        public long QueryId { get; set; }

        [ForeignKey(nameof(QueryId))]
        public virtual SearchQuery Query { get; set; }

        public bool Closed { get; set; }

        public bool Sold { get; set; }

        public bool Hidden { get; set; }

        public bool Notified { get; set; }

        public DateTime? Ends { get; set; }

        public decimal? BidPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? FinalPrice { get; set; }

        public virtual ICollection<ResultExecution> Executions { get; set; } = new HashSet<ResultExecution>();

        public bool New { get; set; }

        public bool Favorite { get; set; }
    }
}
