namespace MiskatonicUniversity.Migrations
{
	using Models;
	using DAL;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<SchoolContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(SchoolContext context)
		{
			var students = new List<Student>
			{
				new Student { FirstMidName = "Frank",   LastName = "Elwood",
					EnrollmentDate = DateTime.Parse("2010-09-01") },
				new Student { FirstMidName = "Walter", LastName = "Gilman",
					EnrollmentDate = DateTime.Parse("2012-09-01") },
				new Student { FirstMidName = "Herbert",   LastName = "West",
					EnrollmentDate = DateTime.Parse("2013-09-01") },
				new Student { FirstMidName = "Dakota",    LastName = "Danford",
					EnrollmentDate = DateTime.Parse("2012-09-01") },
				new Student { FirstMidName = "Gil",      LastName = "Gedney",
					EnrollmentDate = DateTime.Parse("2012-09-01") },
				new Student { FirstMidName = "Chris",    LastName = "Carroll",
					EnrollmentDate = DateTime.Parse("2011-09-01") },
				new Student { FirstMidName = "Rene",    LastName = "Ropes",
					EnrollmentDate = DateTime.Parse("2013-09-01") },
				new Student { FirstMidName = "Bradford",     LastName = "Kenmore",
					EnrollmentDate = DateTime.Parse("2005-09-01") }
			};

			students.ForEach(s => context.Students.AddOrUpdate(p => p.LastName, s));
			context.SaveChanges();

			var instructors = new List<Instructor>
			{
				new Instructor { FirstMidName = "William",     LastName = "Dyer",
					HireDate = DateTime.Parse("1995-03-11") },
				new Instructor { FirstMidName = "Francis",    LastName = "Morgan",
					HireDate = DateTime.Parse("2002-07-06") },
				new Instructor { FirstMidName = "Nathaniel Wingate",   LastName = "Peaslee",
					HireDate = DateTime.Parse("1998-07-01") },
				new Instructor { FirstMidName = "Warren", LastName = "Rice",
					HireDate = DateTime.Parse("2001-01-15") },
				new Instructor { FirstMidName = "Albert",   LastName = "Wilmarth",
					HireDate = DateTime.Parse("2004-02-12") }
			};
			instructors.ForEach(s => context.Instructors.AddOrUpdate(p => p.LastName, s));
			context.SaveChanges();

			var departments = new List<Department>
			{
				new Department { Name = "Geology",     Budget = 350000,
					StartDate = DateTime.Parse("2007-09-01"),
					InstructorID  = instructors.Single( i => i.LastName == "Dyer").ID },
				new Department { Name = "Medicine & Comparative Anatomy", Budget = 100000,
					StartDate = DateTime.Parse("2007-09-01"),
					InstructorID  = instructors.Single( i => i.LastName == "Morgan").ID },
				new Department { Name = "Political Economy", Budget = 350000,
					StartDate = DateTime.Parse("2007-09-01"),
					InstructorID  = instructors.Single( i => i.LastName == "Peaslee").ID },
				new Department { Name = "Classical Languages",   Budget = 100000,
					StartDate = DateTime.Parse("2007-09-01"),
					InstructorID  = instructors.Single( i => i.LastName == "Rice").ID }
			};
			departments.ForEach(s => context.Departments.AddOrUpdate(p => p.Name, s));
			context.SaveChanges();

			var courses = new List<Course>
			{
				new Course {CourseID = 1050, Title = "Constitutional Economics of the Yithian Library City",      Credits = 3,
				  DepartmentID = departments.Single( s => s.Name == "Political Economy").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 4022, Title = "Ethnolinguistics of the Outer Gods", Credits = 3,
				  DepartmentID = departments.Single( s => s.Name == "Classical Languages").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 4041, Title = "The Necronomicon in Translation", Credits = 3,
				  DepartmentID = departments.Single( s => s.Name == "Classical Languages").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 1045, Title = "Psychotropic Compounds & the Mind",       Credits = 4,
				  DepartmentID = departments.Single( s => s.Name == "Medicine & Comparative Anatomy").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 3141, Title = "Extraplanar Morphology",   Credits = 4,
				  DepartmentID = departments.Single( s => s.Name == "Medicine & Comparative Anatomy").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 2021, Title = "Intro to Precambrian Paleontology",    Credits = 3,
				  DepartmentID = departments.Single( s => s.Name == "Geology").DepartmentID,
				  Instructors = new List<Instructor>()
				},
				new Course {CourseID = 2042, Title = "Antarctic Field Expedition",     Credits = 4,
				  DepartmentID = departments.Single( s => s.Name == "Geology").DepartmentID,
				  Instructors = new List<Instructor>()
				},
			};
			courses.ForEach(s => context.Courses.AddOrUpdate(p => p.CourseID, s));
			context.SaveChanges();

			var officeAssignments = new List<OfficeAssignment>
			{
				new OfficeAssignment {
					InstructorID = instructors.Single( i => i.LastName == "Morgan").ID,
					Location = "Allan Halsey School of Medicine, Suite 17" },
				new OfficeAssignment {
					InstructorID = instructors.Single( i => i.LastName == "Peaslee").ID,
					Location = "Ashley-Delapore Hall, Room 27" },
				new OfficeAssignment {
					InstructorID = instructors.Single( i => i.LastName == "Rice").ID,
					Location = "304 Armitage Library" },
			};
			officeAssignments.ForEach(s => context.OfficeAssignments.AddOrUpdate(p => p.InstructorID, s));
			context.SaveChanges();

			AddOrUpdateInstructor(context, "The Necronomicon in Translation", "Rice");
			AddOrUpdateInstructor(context, "Constitutional Economics of the Yithian Library City", "Peaslee");
			AddOrUpdateInstructor(context, "Ethnolinguistics of the Outer Gods", "Wilmarth");
			AddOrUpdateInstructor(context, "The Necronomicon in Translation", "Wilmarth");
			AddOrUpdateInstructor(context, "Psychotropic Compounds & the Mind", "Morgan");
			AddOrUpdateInstructor(context, "Extraplanar Morphology", "Morgan");
			AddOrUpdateInstructor(context, "Intro to Precambrian Paleontology", "Dyer");
			AddOrUpdateInstructor(context, "Antarctic Field Expedition", "Dyer");

			context.SaveChanges();

			var enrollments = new List<Enrollment>
			{
				new Enrollment {
					StudentID = students.Single(s => s.LastName == "Elwood").ID,
					CourseID = courses.Single(c => c.Title == "Constitutional Economics of the Yithian Library City" ).CourseID,
					Grade = Grade.A
				},
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "Elwood").ID,
					CourseID = courses.Single(c => c.Title == "Ethnolinguistics of the Outer Gods" ).CourseID,
					Grade = Grade.C
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "Elwood").ID,
					CourseID = courses.Single(c => c.Title == "The Necronomicon in Translation" ).CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					 StudentID = students.Single(s => s.LastName == "Gilman").ID,
					CourseID = courses.Single(c => c.Title == "Psychotropic Compounds & the Mind" ).CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					 StudentID = students.Single(s => s.LastName == "Gilman").ID,
					CourseID = courses.Single(c => c.Title == "Extraplanar Morphology" ).CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "Gilman").ID,
					CourseID = courses.Single(c => c.Title == "Intro to Precambrian Paleontology" ).CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "West").ID,
					CourseID = courses.Single(c => c.Title == "Constitutional Economics of the Yithian Library City" ).CourseID
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "West").ID,
					CourseID = courses.Single(c => c.Title == "Ethnolinguistics of the Outer Gods").CourseID,
					Grade = Grade.B
				 },
				new Enrollment {
					StudentID = students.Single(s => s.LastName == "Danford").ID,
					CourseID = courses.Single(c => c.Title == "Constitutional Economics of the Yithian Library City").CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "Gedney").ID,
					CourseID = courses.Single(c => c.Title == "Intro to Precambrian Paleontology").CourseID,
					Grade = Grade.B
				 },
				 new Enrollment {
					StudentID = students.Single(s => s.LastName == "Carroll").ID,
					CourseID = courses.Single(c => c.Title == "Antarctic Field Expedition").CourseID,
					Grade = Grade.B
				 }
			};

			foreach (Enrollment e in enrollments)
			{
				var enrollmentInDataBase = context.Enrollments.Where(
					s =>
						 s.Student.ID == e.StudentID &&
						 s.Course.CourseID == e.CourseID).SingleOrDefault();
				if (enrollmentInDataBase == null)
				{
					context.Enrollments.Add(e);
				}
			}
			context.SaveChanges();
		}

		void AddOrUpdateInstructor(SchoolContext context, string courseTitle, string instructorName)
		{
			var crs = context.Courses.SingleOrDefault(c => c.Title == courseTitle);
			var inst = crs.Instructors.SingleOrDefault(i => i.LastName == instructorName);
			if (inst == null)
				crs.Instructors.Add(context.Instructors.Single(i => i.LastName == instructorName));
		}
	}
}