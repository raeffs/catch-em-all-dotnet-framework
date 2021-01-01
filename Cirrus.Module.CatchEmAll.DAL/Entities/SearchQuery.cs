using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("SearchQueries", Schema = "catchemall")]
    public class SearchQuery : IHasIndex, IForTenant
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<SearchResult> Results { get; set; } = new HashSet<SearchResult>();

        public bool UseDescription { get; set; }

        public string WithAllTheseWords { get; set; }

        public string WithOneOfTheseWords { get; set; }

        public string WithExactlyTheseWords { get; set; }

        public string WithNoneOfTheseWords { get; set; }

        public long CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();

        public virtual ICollection<QueryExecution> Executions { get; set; } = new HashSet<QueryExecution>();

        public bool AutoFilterDeletedDuplicates { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        public string TagValues { get; set; }

        public bool EnableNotifications { get; set; }

        public NotificationMode NotificationMode { get; set; }

        public decimal? DesiredPrice { get; set; }

        public bool Hidden { get; set; }

        public Condition Condition { get; set; }
    }
}
