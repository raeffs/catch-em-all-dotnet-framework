using System.Data.Entity;
using Cirrus.DAL;

namespace Cirrus.Module.CatchEmAll.DAL
{
    public class CatchEmAllExtendingContext : ExtendingContext
    {
        public override void ModelBuilding(DbModelBuilder modelBuilder) => CatchEmAllEntityContext.ModelBuilding(modelBuilder);

        internal static void SetAddHandlers()
        {
        }
    }
}
