namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mims",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MimID = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Creator = c.String(nullable: false),
                        Image = c.String(nullable: false),
                        CaptionTop = c.String(),
                        CaptionBottom = c.String(),
                        NextMimID = c.Int(nullable: false),
                        PrevMimID = c.Int(nullable: false),
                        MimSeqID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.MimSeqs", t => t.MimSeqID, cascadeDelete: true)
                .Index(t => t.MimSeqID);
            
            CreateTable(
                "dbo.MimSeqs",
                c => new
                    {
                        MimSeqID = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MimSeqID);
            
            CreateTable(
                "dbo.MimSeqLikes",
                c => new
                    {
                        MimSeqID = c.Guid(nullable: false),
                        User = c.String(nullable: false, maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MimSeqID, t.User });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mims", "MimSeqID", "dbo.MimSeqs");
            DropIndex("dbo.Mims", new[] { "MimSeqID" });
            DropTable("dbo.MimSeqLikes");
            DropTable("dbo.MimSeqs");
            DropTable("dbo.Mims");
        }
    }
}
