namespace Poordooytify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsongkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "Key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "Key");
        }
    }
}
