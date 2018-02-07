using System;
using System.Collections.Generic;
using MiskatonicUniversity.Models;

namespace MiskatonicUniversity.DAL
{
	// causes dB to created as needed and loads test data into the new dB
	public class SchoolInitializer : System.Data.Entity. DropCreateDatabaseIfModelChanges<SchoolContext>
	{
		// seed takes dB context object as input param
			// code in method uses that object to add new entities to the dB
		protected override void Seed(SchoolContext context)
		{
			var students = new List<Student>
			{
				new Student { FirstMidName="Frank",LastName="Elwood",EnrollmentDate=DateTime.Parse("2005-09-01")},
				new Student { FirstMidName="Walter",LastName="Gilman",EnrollmentDate=DateTime.Parse("2002-09-01")},
				new Student { FirstMidName="Herbert",LastName="West",EnrollmentDate=DateTime.Parse("2003-09-01")},
				new Student { FirstMidName="Dakota",LastName="Danford",EnrollmentDate=DateTime.Parse("2002-09-01")},
				new Student { FirstMidName="Gil",LastName="Gedney",EnrollmentDate=DateTime.Parse("2002-09-01")},
				new Student { FirstMidName="Chris",LastName="Carroll",EnrollmentDate=DateTime.Parse("2001-09-01")},
				new Student { FirstMidName="Rene",LastName="Ropes",EnrollmentDate=DateTime.Parse("2003-09-01")},
				new Student { FirstMidName="Bradford",LastName="Kenmore",EnrollmentDate=DateTime.Parse("2005-09-01")}
			};
		
			students.ForEach(s => context.Students.Add(s));
			context.SaveChanges();

			var courses = new List<Course>
			{
				new Course { CourseID=1050,Title="Constitutional Economics of the Yithian Library City",Credits=3,},
				new Course { CourseID=4022,Title="Ethnolinguistics of the Outer Gods",Credits=3,},
				new Course { CourseID=4041,Title="The Necronomicon in Translation",Credits=3,},
				new Course { CourseID=1045,Title="Psychotropic Compounds & the Mind",Credits=4,},
				new Course { CourseID=3141,Title="Extraplanar Morphology",Credits=4,},
				new Course { CourseID=2021,Title="Intro to Precambrian Paleontology",Credits=3,},
				new Course { CourseID=2042,Title="Antarctic Field Expedition",Credits=4,},
			};

			courses.ForEach(s => context.Courses.Add(s));
			context.SaveChanges();

			var enrollments = new List<Enrollment>
			{
				new Enrollment {StudentID=1,CourseID=1050,Grade=Grade.A },
				new Enrollment {StudentID=1,CourseID=4022,Grade=Grade.C },
				new Enrollment {StudentID=1,CourseID=4041,Grade=Grade.B },
				new Enrollment {StudentID=2,CourseID=1045,Grade=Grade.B },
				new Enrollment {StudentID=2,CourseID=3141,Grade=Grade.F },
				new Enrollment {StudentID=2,CourseID=2021,Grade=Grade.F },
				new Enrollment {StudentID=3,CourseID=1050 },
				new Enrollment {StudentID=4,CourseID=1050 },
				new Enrollment {StudentID=4,CourseID=4022,Grade=Grade.F },
				new Enrollment {StudentID=5,CourseID=4041,Grade=Grade.C },
				new Enrollment {StudentID=6,CourseID=1045 },
				new Enrollment {StudentID=7,CourseID=3141,Grade=Grade.A },
			};

			enrollments.ForEach(s => context.Enrollments.Add(s));
			context.SaveChanges();
		}
	}
}