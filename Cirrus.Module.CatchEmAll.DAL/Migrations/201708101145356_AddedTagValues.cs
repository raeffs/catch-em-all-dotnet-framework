namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedTagValues : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchQueries", "TagValues", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchQueries", "TagValues");
        }
    }
}
