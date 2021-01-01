using Cirrus.Module.CatchEmAll.DAL.Entities;

namespace Cirrus.Module.CatchEmAll.Models
{
    public class ScheduleOptions
    {
        public long ScheduleId { get; set; }

        public bool IsDefault { get; set; }

        public long UpdateIntervalInMinutes { get; set; }

        public ExecutionType Type { get; set; }
    }
}
