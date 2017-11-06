using Cirrus.Attributes;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    public enum FeedbackStatus
    {
        [Color("orange")]
        New = 0,

        [Color("lightskyblue")]
        Accepted = 1,

        [Color("indianred")]
        Rejected = 2,

        [Color("lightgreen")]
        Implemented = 3
    }
}
