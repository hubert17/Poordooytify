namespace Poordooytify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmOod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Moods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 30),
                        Name = c.String(),
                        Description = c.String(),
                        Categories = c.String(),
                        InActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Moods", new[] { "Key" });
            DropTable("dbo.Moods");
        }
    }
}
