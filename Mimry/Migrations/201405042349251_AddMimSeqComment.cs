namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMimSeqComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MimSeqCommentLikes",
                c => new
                    {
                        MimSeqCommentID = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MimSeqCommentID, t.User });
            
            CreateTable(
                "dbo.MimSeqComments",
                c => new
                    {
                        CommentID = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        User = c.String(nullable: false),
                        Value = c.String(nullable: false),
                        MimSeq_MimSeqID = c.Guid(nullable: false),
                        Parent_CommentID = c.Int(),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.MimSeqs", t => t.MimSeq_MimSeqID, cascadeDelete: true)
                .ForeignKey("dbo.MimSeqComments", t => t.Parent_CommentID)
                .Index(t => t.MimSeq_MimSeqID)
                .Index(t => t.Parent_CommentID);
            
            CreateTable(
                "dbo.MimSeqCommentVotes",
                c => new
                    {
                        MimSeqCommentID = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 128),
                        Vote = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MimSeqCommentID, t.User });
            
            AddColumn("dbo.MimSeqLikes", "LastModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.MimVotes", "LastModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MimSeqComments", "Parent_CommentID", "dbo.MimSeqComments");
            DropForeignKey("dbo.MimSeqComments", "MimSeq_MimSeqID", "dbo.MimSeqs");
            DropIndex("dbo.MimSeqComments", new[] { "Parent_CommentID" });
            DropIndex("dbo.MimSeqComments", new[] { "MimSeq_MimSeqID" });
            DropColumn("dbo.MimVotes", "LastModifiedDate");
            DropColumn("dbo.MimSeqLikes", "LastModifiedDate");
            DropTable("dbo.MimSeqCommentVotes");
            DropTable("dbo.MimSeqComments");
            DropTable("dbo.MimSeqCommentLikes");
        }
    }
}
