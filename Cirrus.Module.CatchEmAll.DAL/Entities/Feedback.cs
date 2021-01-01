using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("Feedbacks", Schema = "catchemall")]
    public class Feedback : IHasIndex, IForTenant, IForUser
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserReference User { get; set; }

        public FeedbackStatus Status { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }
    }
}
