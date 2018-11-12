namespace K9.DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserMembership : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserMembership",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        MembershipOptionId = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MembershipOption", t => t.MembershipOptionId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.MembershipOptionId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMembership", "UserId", "dbo.User");
            DropForeignKey("dbo.UserMembership", "MembershipOptionId", "dbo.MembershipOption");
            DropIndex("dbo.UserMembership", new[] { "Name" });
            DropIndex("dbo.UserMembership", new[] { "MembershipOptionId" });
            DropIndex("dbo.UserMembership", new[] { "UserId" });
            DropTable("dbo.UserMembership");
        }
    }
}
