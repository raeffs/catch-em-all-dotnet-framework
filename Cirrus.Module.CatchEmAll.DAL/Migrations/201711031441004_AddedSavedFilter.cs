namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedSavedFilter : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "catchemall.SavedFilters",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    WorkflowStepId = c.String(),
                    OrderByProperties = c.String(),
                    OrderByAscending = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.UserReferences", t => t.UserId)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            this.DropForeignKey("catchemall.SavedFilters", "UserId", "catchemall.UserReferences");
            this.DropIndex("catchemall.SavedFilters", new[] { "UserId" });
            this.DropTable("catchemall.SavedFilters");
        }
    }
}
