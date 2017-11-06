namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedHiddenField : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchQueries", "Hidden", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchQueries", "Hidden");
        }
    }
}
