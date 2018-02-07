namespace MiskatonicUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DebugComplexDataModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Student", "LastName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Student", "LastName", c => c.String(maxLength: 50));
        }
    }
}
