namespace Poordooytify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniqueSongkey : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Songs", "Key", c => c.String(maxLength: 30));
            CreateIndex("dbo.Songs", "Key", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Songs", new[] { "Key" });
            AlterColumn("dbo.Songs", "Key", c => c.String());
        }
    }
}
