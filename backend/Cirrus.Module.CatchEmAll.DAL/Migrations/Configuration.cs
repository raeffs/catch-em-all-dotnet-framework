namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CatchEmAllEntityContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = false;
            this.ContextKey = "catchemall";
        }

        protected override void Seed(CatchEmAllEntityContext context)
        {
            Seeder.Seed(context);
        }
    }
}
