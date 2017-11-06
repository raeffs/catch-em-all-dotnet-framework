namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedNotificationModes : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchQueries", "NotificationMode", c => c.Int(nullable: false));
            this.AddColumn("catchemall.SearchQueries", "DesiredPrice", c => c.Decimal(precision: 18, scale: 6));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchQueries", "DesiredPrice");
            this.DropColumn("catchemall.SearchQueries", "NotificationMode");
        }
    }
}
