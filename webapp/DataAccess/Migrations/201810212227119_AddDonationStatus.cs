namespace K9.DataAccessLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDonationStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donation", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donation", "Status");
        }
    }
}
