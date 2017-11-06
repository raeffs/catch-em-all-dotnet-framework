namespace Cirrus.Module.CatchEmAll.Models
{
    internal class ScheduleStatistic
    {
        public int Total { get; set; }

        public int Enabled { get; set; }

        public int Disabled { get; set; }

        public int MissingSchedule { get; set; }

        public int OutdatedSchedule { get; set; }
    }
}
