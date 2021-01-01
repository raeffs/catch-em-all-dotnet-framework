using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("Settings", Schema = "catchemall")]
    public class Settings : IHasIndex, IForTenant, IForUser
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserReference User { get; set; }

        public string Email { get; set; }

        public string AlternativeEmail { get; set; }

        public string IftttMakerKey { get; set; }

        public string IftttMakerEventName { get; set; }

        public bool EnableEmailNotification { get; set; }

        public bool EnableIftttNotification { get; set; }

        public bool EnableNotificationsDefault { get; set; }

        public bool AutoFilterDeletedDuplicatesDefault { get; set; }
    }
}
