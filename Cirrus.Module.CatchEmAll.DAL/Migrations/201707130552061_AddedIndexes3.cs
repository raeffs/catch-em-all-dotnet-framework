namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedIndexes3 : DbMigration
    {
        public override void Up()
        {
            this.DropIndex("catchemall.QueryExecutions", "IX_QueryIdStartSuccessful");
            this.CreateIndex("catchemall.QueryExecutions", new[] { "TenantId", "Start", "Successful" }, name: "IX_TenantIdStartSuccessful");
        }

        public override void Down()
        {
            this.DropIndex("catchemall.QueryExecutions", "IX_TenantIdStartSuccessful");
            this.CreateIndex("catchemall.QueryExecutions", new[] { "QueryId", "Start", "Successful" }, name: "IX_QueryIdStartSuccessful");
        }
    }
}
