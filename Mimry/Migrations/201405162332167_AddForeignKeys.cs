namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MimSeqCommentVotes", "MimSeqCommentID");
            CreateIndex("dbo.MimSeqLikes", "MimSeqID");
            CreateIndex("dbo.MimVotes", "MimID");
            AddForeignKey("dbo.MimSeqCommentVotes", "MimSeqCommentID", "dbo.MimSeqComments", "CommentID", cascadeDelete: true);
            AddForeignKey("dbo.MimSeqLikes", "MimSeqID", "dbo.MimSeqs", "MimSeqID", cascadeDelete: true);
            AddForeignKey("dbo.MimVotes", "MimID", "dbo.Mims", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MimVotes", "MimID", "dbo.Mims");
            DropForeignKey("dbo.MimSeqLikes", "MimSeqID", "dbo.MimSeqs");
            DropForeignKey("dbo.MimSeqCommentVotes", "MimSeqCommentID", "dbo.MimSeqComments");
            DropIndex("dbo.MimVotes", new[] { "MimID" });
            DropIndex("dbo.MimSeqLikes", new[] { "MimSeqID" });
            DropIndex("dbo.MimSeqCommentVotes", new[] { "MimSeqCommentID" });
        }
    }
}
