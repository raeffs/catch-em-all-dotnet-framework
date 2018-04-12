namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedConditions : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.SearchQueries", "Condition", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("catchemall.SearchQueries", "Condition");
        }
    }
}
