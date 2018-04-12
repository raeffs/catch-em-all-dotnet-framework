using System;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Flags]
    public enum Condition
    {
        NewSeeDescription = 1,
        NewSealed = 2,
        Used = 8,
        Defect = 16,
        Ancient = 32
    }
}
