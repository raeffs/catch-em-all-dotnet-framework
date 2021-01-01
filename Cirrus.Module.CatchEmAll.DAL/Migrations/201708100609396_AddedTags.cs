namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedTags : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("catchemall.Categories", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Feedbacks", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Settings", "UserId", "catchemall.UserReferences");
            this.CreateTable(
                "catchemall.Tags",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.UserReferences", t => t.UserId)
                .Index(t => t.UserId);

            this.CreateTable(
                "catchemall.SearchQueryTags",
                c => new
                {
                    SearchQueryId = c.Long(nullable: false),
                    TagId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.SearchQueryId, t.TagId })
                .ForeignKey("catchemall.SearchQueries", t => t.SearchQueryId, cascadeDelete: true)
                .ForeignKey("catchemall.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.SearchQueryId)
                .Index(t => t.TagId);

            this.AddForeignKey("catchemall.Categories", "UserId", "catchemall.UserReferences", "Id");
            this.AddForeignKey("catchemall.Feedbacks", "UserId", "catchemall.UserReferences", "Id");
            this.AddForeignKey("catchemall.Settings", "UserId", "catchemall.UserReferences", "Id");
        }

        public override void Down()
        {
            this.DropForeignKey("catchemall.Settings", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Feedbacks", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Categories", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.SearchQueryTags", "TagId", "catchemall.Tags");
            this.DropForeignKey("catchemall.SearchQueryTags", "SearchQueryId", "catchemall.SearchQueries");
            this.DropForeignKey("catchemall.Tags", "UserId", "catchemall.UserReferences");
            this.DropIndex("catchemall.SearchQueryTags", new[] { "TagId" });
            this.DropIndex("catchemall.SearchQueryTags", new[] { "SearchQueryId" });
            this.DropIndex("catchemall.Tags", new[] { "UserId" });
            this.DropTable("catchemall.SearchQueryTags");
            this.DropTable("catchemall.Tags");
            this.AddForeignKey("catchemall.Settings", "UserId", "catchemall.UserReferences", "Id", cascadeDelete: true);
            this.AddForeignKey("catchemall.Feedbacks", "UserId", "catchemall.UserReferences", "Id", cascadeDelete: true);
            this.AddForeignKey("catchemall.Categories", "UserId", "catchemall.UserReferences", "Id", cascadeDelete: true);
        }
    }
}
