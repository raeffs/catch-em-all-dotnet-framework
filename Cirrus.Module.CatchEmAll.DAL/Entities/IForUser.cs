using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cirrus.Module.CatchEmAll.DAL.Entities
{
    public interface IForUser
    {
        long UserId { get; }

        UserReference User { get; }
    }
}
