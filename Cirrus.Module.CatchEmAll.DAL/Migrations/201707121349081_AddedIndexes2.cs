namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedIndexes2 : DbMigration
    {
        public override void Up()
        {
            this.CreateIndex("catchemall.QueryExecutions", new[] { "QueryId", "Start", "Successful" }, name: "IX_QueryIdStartSuccessful");
        }

        public override void Down()
        {
            this.DropIndex("catchemall.QueryExecutions", "IX_QueryIdStartSuccessful");
        }
    }
}
