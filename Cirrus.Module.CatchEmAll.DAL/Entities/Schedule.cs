using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("Schedules", Schema = "catchemall")]
    public class Schedule : IHasIndex, IForTenant
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public Guid? ScheduleId { get; set; }

        public string CronExpression { get; set; }

        public ExecutionType Type { get; set; }

        public long UpdateIntervalInMinutes { get; set; }

        public bool IsDefault { get; set; }

        public bool IsEnabled { get; set; }

        public virtual ICollection<SearchQuery> Queries { get; set; } = new HashSet<SearchQuery>();
    }
}
