namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewsItemShowLocalOnly : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewsItem", "IsShowLocalOnly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsItem", "IsShowLocalOnly");
        }
    }
}
