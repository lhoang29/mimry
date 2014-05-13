namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteMimSeqCommentLike : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.MimSeqCommentLikes");
        }
        
        public override void Down()
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
            
        }
    }
}
