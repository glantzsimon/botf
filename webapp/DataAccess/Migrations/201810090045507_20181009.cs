namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181009 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contact", "FullName", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contact", "FullName", c => c.String(maxLength: 128));
        }
    }
}
