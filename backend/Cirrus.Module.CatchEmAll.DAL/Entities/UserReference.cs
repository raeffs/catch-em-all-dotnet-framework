using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    [Table("UserReferences", Schema = "catchemall")]
    public class UserReference : IHasIndex, IForTenant
    {
        public long TenantId { get; set; }

        public long Id { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; }
    }
}
