namespace K9.DataAccessLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ExtendCustomr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contact", "UserId", c => c.Int(true));
            AddColumn("dbo.Contact", "StripeCustomerId", c => c.String());
            CreateIndex("dbo.Contact", "UserId");
            AddForeignKey("dbo.Contact", "UserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contact", "UserId", "dbo.User");
            DropIndex("dbo.Contact", new[] { "UserId" });
            DropColumn("dbo.Contact", "StripeCustomerId");
            DropColumn("dbo.Contact", "UserId");
        }
    }
}
