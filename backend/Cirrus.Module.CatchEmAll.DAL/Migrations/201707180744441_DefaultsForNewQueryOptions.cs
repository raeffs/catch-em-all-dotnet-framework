namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DefaultsForNewQueryOptions : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.Settings", "EnableNotificationsDefault", c => c.Boolean(nullable: false, defaultValue: true));
            this.AddColumn("catchemall.Settings", "AutoFilterDeletedDuplicatesDefault", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.Settings", "AutoFilterDeletedDuplicatesDefault");
            this.DropColumn("catchemall.Settings", "EnableNotificationsDefault");
        }
    }
}
