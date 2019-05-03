namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeoFriendlyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArchiveItem", "SeoFriendlyId", c => c.String());
            AddColumn("dbo.NewsItem", "SeoFriendlyId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsItem", "SeoFriendlyId");
            DropColumn("dbo.ArchiveItem", "SeoFriendlyId");
        }
    }
}
