namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArchiveItem", "ImageUrl", c => c.String(maxLength: 512));
            AddColumn("dbo.ArchiveItem", "AdditionalCssClasses", c => c.String());
            AddColumn("dbo.NewsItem", "ImageUrl", c => c.String(maxLength: 512));
            AddColumn("dbo.NewsItem", "AdditionalCssClasses", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsItem", "AdditionalCssClasses");
            DropColumn("dbo.NewsItem", "ImageUrl");
            DropColumn("dbo.ArchiveItem", "AdditionalCssClasses");
            DropColumn("dbo.ArchiveItem", "ImageUrl");
        }
    }
}
