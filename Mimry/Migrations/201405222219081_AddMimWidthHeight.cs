namespace Mimry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMimWidthHeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mims", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Mims", "Height", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mims", "Height");
            DropColumn("dbo.Mims", "Width");
        }
    }
}
