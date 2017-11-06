namespace Cirrus.Module.CatchEmAll.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedIfttt : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("catchemall.Settings", "IftttMakerEventName", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("catchemall.Settings", "IftttMakerEventName");
        }
    }
}
