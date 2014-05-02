namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMimVote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MimVotes",
                c => new
                    {
                        MimID = c.Int(nullable: false),
                        User = c.String(nullable: false, maxLength: 128),
                        Vote = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MimID, t.User });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MimVotes");
        }
    }
}
