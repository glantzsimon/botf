namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_AutoRenew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMembership", "IsAutoRenew", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMembership", "IsAutoRenew");
        }
    }
}
