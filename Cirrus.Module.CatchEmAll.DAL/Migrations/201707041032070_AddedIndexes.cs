namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedIndexes : DbMigration
    {
        public override void Up()
        {
            this.DropIndex("catchemall.QueryExecutions", new[] { "QueryId" });
            this.DropIndex("catchemall.ResultExecutions", new[] { "ResultId" });

            // this.CreateIndex("catchemall.QueryExecutions", new[] { "QueryId", "Successful" }, name: "IX_QueryIdSuccessful");
            this.Sql(@"CREATE NONCLUSTERED INDEX [IX_QueryIdSuccessful] ON [catchemall].[QueryExecutions] ([QueryId], [Successful]) INCLUDE ([End])");

            this.CreateIndex("catchemall.QueryExecutions", "QueryId");

            // this.CreateIndex("catchemall.ResultExecutions", new[] { "ResultId", "Successful" }, name: "IX_ResultIdSuccessful");
            this.Sql(@"CREATE NONCLUSTERED INDEX [IX_ResultIdSuccessful] ON [catchemall].[ResultExecutions] ([ResultId], [Successful]) INCLUDE ([End])");

            this.CreateIndex("catchemall.ResultExecutions", "ResultId");
        }

        public override void Down()
        {
            this.DropIndex("catchemall.ResultExecutions", new[] { "ResultId" });
            this.DropIndex("catchemall.ResultExecutions", "IX_ResultIdSuccessful");
            this.DropIndex("catchemall.QueryExecutions", new[] { "QueryId" });
            this.DropIndex("catchemall.QueryExecutions", "IX_QueryIdSuccessful");
            this.CreateIndex("catchemall.ResultExecutions", "ResultId");
            this.CreateIndex("catchemall.QueryExecutions", "QueryId");
        }
    }
}
