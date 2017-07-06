namespace Poordooytify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSongmood : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SongMoods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SongKey = c.String(maxLength: 30),
                        MoodKey = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.SongKey, t.MoodKey }, unique: true, name: "IX_FirstAndSecond");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SongMoods", "IX_FirstAndSecond");
            DropTable("dbo.SongMoods");
        }
    }
}
