namespace Poordooytify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletesongkey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Songs", new[] { "Key" });
            DropColumn("dbo.Songs", "Key");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Songs", "Key", c => c.String());
            CreateIndex("dbo.Songs", "Key", unique: true);
        }
    }
}
