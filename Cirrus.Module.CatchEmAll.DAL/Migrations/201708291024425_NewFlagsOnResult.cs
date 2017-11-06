namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NewFlagsOnResult : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchResults", "New", c => c.Boolean(nullable: false));
            this.AddColumn("catchemall.SearchResults", "Favorite", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchResults", "Favorite");
            this.DropColumn("catchemall.SearchResults", "New");
        }
    }
}
