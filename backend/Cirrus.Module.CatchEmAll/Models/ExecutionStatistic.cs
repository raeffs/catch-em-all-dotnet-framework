using System;

namespace Cirrus.Module.CatchEmAll.Models
{
    internal class ExecutionStatistic
    {
        public DateTime? Day { get; set; }
        public DateTime? Hour { get; set; }
        public DateTime? Minute { get; set; }
        public int Total { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
    }
}
