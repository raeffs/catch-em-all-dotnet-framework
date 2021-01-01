namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedMessageToExecution : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.QueryExecutions", "Message", c => c.String());
            this.AddColumn("catchemall.ResultExecutions", "Message", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("catchemall.ResultExecutions", "Message");
            this.DropColumn("catchemall.QueryExecutions", "Message");
        }
    }
}
