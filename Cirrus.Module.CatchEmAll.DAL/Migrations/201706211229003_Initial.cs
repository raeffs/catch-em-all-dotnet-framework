namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "catchemall.Categories",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    Number = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                    UserId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.UserReferences", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            this.CreateTable(
                "catchemall.SearchQueries",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    Name = c.String(),
                    UseDescription = c.Boolean(nullable: false),
                    WithAllTheseWords = c.String(),
                    WithOneOfTheseWords = c.String(),
                    WithExactlyTheseWords = c.String(),
                    WithNoneOfTheseWords = c.String(),
                    CategoryId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);

            this.CreateTable(
                "catchemall.QueryExecutions",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    Start = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    End = c.DateTime(precision: 7, storeType: "datetime2"),
                    Successful = c.Boolean(nullable: false),
                    IsUserInitiated = c.Boolean(nullable: false),
                    QueryId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.SearchQueries", t => t.QueryId, cascadeDelete: true)
                .Index(t => t.QueryId);

            this.CreateTable(
                "catchemall.SearchResults",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    ExternalId = c.Long(nullable: false),
                    Name = c.String(nullable: false),
                    Description = c.String(),
                    QueryId = c.Long(nullable: false),
                    Closed = c.Boolean(nullable: false),
                    Sold = c.Boolean(nullable: false),
                    Hidden = c.Boolean(nullable: false),
                    Notified = c.Boolean(nullable: false),
                    Ends = c.DateTime(precision: 7, storeType: "datetime2"),
                    BidPrice = c.Decimal(precision: 18, scale: 6),
                    PurchasePrice = c.Decimal(precision: 18, scale: 6),
                    FinalPrice = c.Decimal(precision: 18, scale: 6),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.SearchQueries", t => t.QueryId, cascadeDelete: true)
                .Index(t => new { t.ExternalId, t.QueryId }, unique: true);

            this.CreateTable(
                "catchemall.ResultExecutions",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    Start = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    End = c.DateTime(precision: 7, storeType: "datetime2"),
                    Successful = c.Boolean(nullable: false),
                    IsUserInitiated = c.Boolean(nullable: false),
                    ResultId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.SearchResults", t => t.ResultId, cascadeDelete: true)
                .Index(t => t.ResultId);

            this.CreateTable(
                "catchemall.Schedules",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    ScheduleId = c.Guid(),
                    CronExpression = c.String(),
                    Type = c.Int(nullable: false),
                    UpdateIntervalInMinutes = c.Long(nullable: false),
                    IsDefault = c.Boolean(nullable: false),
                    IsEnabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            this.CreateTable(
                "catchemall.UserReferences",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    UserId = c.Guid(nullable: false),
                    Username = c.String(),
                })
                .PrimaryKey(t => t.Id);

            this.CreateTable(
                "catchemall.Feedbacks",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    Status = c.Int(nullable: false),
                    Name = c.String(),
                    Description = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.UserReferences", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            this.CreateTable(
                "catchemall.Settings",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Long(nullable: false),
                    UserId = c.Long(nullable: false),
                    Email = c.String(),
                    AlternativeEmail = c.String(),
                    IftttMakerKey = c.String(),
                    EnableEmailNotification = c.Boolean(nullable: false),
                    EnableIftttNotification = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("catchemall.UserReferences", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            this.CreateTable(
                "catchemall.SearchQuerySchedules",
                c => new
                {
                    SearchQueryId = c.Long(nullable: false),
                    ScheduleId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.SearchQueryId, t.ScheduleId })
                .ForeignKey("catchemall.SearchQueries", t => t.SearchQueryId, cascadeDelete: true)
                .ForeignKey("catchemall.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .Index(t => t.SearchQueryId)
                .Index(t => t.ScheduleId);
        }

        public override void Down()
        {
            this.DropForeignKey("catchemall.Settings", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Feedbacks", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.Categories", "UserId", "catchemall.UserReferences");
            this.DropForeignKey("catchemall.SearchQuerySchedules", "ScheduleId", "catchemall.Schedules");
            this.DropForeignKey("catchemall.SearchQuerySchedules", "SearchQueryId", "catchemall.SearchQueries");
            this.DropForeignKey("catchemall.SearchResults", "QueryId", "catchemall.SearchQueries");
            this.DropForeignKey("catchemall.ResultExecutions", "ResultId", "catchemall.SearchResults");
            this.DropForeignKey("catchemall.QueryExecutions", "QueryId", "catchemall.SearchQueries");
            this.DropForeignKey("catchemall.SearchQueries", "CategoryId", "catchemall.Categories");
            this.DropIndex("catchemall.SearchQuerySchedules", new[] { "ScheduleId" });
            this.DropIndex("catchemall.SearchQuerySchedules", new[] { "SearchQueryId" });
            this.DropIndex("catchemall.Settings", new[] { "UserId" });
            this.DropIndex("catchemall.Feedbacks", new[] { "UserId" });
            this.DropIndex("catchemall.ResultExecutions", new[] { "ResultId" });
            this.DropIndex("catchemall.SearchResults", new[] { "ExternalId", "QueryId" });
            this.DropIndex("catchemall.QueryExecutions", new[] { "QueryId" });
            this.DropIndex("catchemall.SearchQueries", new[] { "CategoryId" });
            this.DropIndex("catchemall.Categories", new[] { "UserId" });
            this.DropTable("catchemall.SearchQuerySchedules");
            this.DropTable("catchemall.Settings");
            this.DropTable("catchemall.Feedbacks");
            this.DropTable("catchemall.UserReferences");
            this.DropTable("catchemall.Schedules");
            this.DropTable("catchemall.ResultExecutions");
            this.DropTable("catchemall.SearchResults");
            this.DropTable("catchemall.QueryExecutions");
            this.DropTable("catchemall.SearchQueries");
            this.DropTable("catchemall.Categories");
        }
    }
}
