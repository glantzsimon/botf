namespace K9.DataAccessLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class K9UpdateJan01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Gender");
        }
    }
}
