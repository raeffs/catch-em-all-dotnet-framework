namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NewQueryOptions : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchQueries", "EnableNotifications", c => c.Boolean(nullable: false, defaultValue: true));
            this.AddColumn("catchemall.SearchQueries", "AutoFilterDeletedDuplicates", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchQueries", "AutoFilterDeletedDuplicates");
            this.DropColumn("catchemall.SearchQueries", "EnableNotifications");
        }
    }
}
