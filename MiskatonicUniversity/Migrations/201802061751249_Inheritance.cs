namespace MiskatonicUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inheritance : DbMigration
    {
        public override void Up()
        {
			// drop foreign keys and indexes that point to tables we're going to drop
			DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");
			DropIndex("dbo.Enrollment", new[] { "StudentID" });
			
			// rename Instructor table as Person table
            RenameTable(name: "dbo.Instructor", newName: "Person");

			// add Student-unique nullable EnrollmentDate property
			AddColumn("dbo.Person", "EnrollmentDate", c => c.DateTime());

			// add Discriminator to distinguish between Person derived classes
            AddColumn("dbo.Person", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Instructor"));

			// make Instructor-unique HireDate property nullable
			AlterColumn("dbo.Person", "HireDate", c => c.DateTime());

			// add temporary key to facilitate movement of records from Student to Person table
			AddColumn("dbo.Person", "OldId", c => c.Int(nullable: true));

			// copy existing Student data into new Person table
			Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, ID AS OldId FROM dbo.Student");

			// fix up existing relationships to match new PKs
			Sql("UPDATE dbo.Enrollment SET StudentId = (SELECT ID FROM dbo.Person WHERE OldId = Enrollment.StudentId AND Discriminator = 'Student')");

			// remove temporary key
			DropColumn("dbo.Person", "OldId");

			// remove Student table
			DropTable("dbo.Student");

			// recreate FKs and indexes pointing to new Person table
			AddForeignKey("dbo.Enrollment", "StudentID", "dbo.Person", "ID", cascadeDelete: true);
			CreateIndex("dbo.Enrollment", "StudentID");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.Person", "HireDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Person", "Discriminator");
            DropColumn("dbo.Person", "EnrollmentDate");
            RenameTable(name: "dbo.Person", newName: "Instructor");
        }
    }
}
