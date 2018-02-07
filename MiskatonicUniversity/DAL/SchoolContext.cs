using MiskatonicUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MiskatonicUniversity.DAL
{
	public class SchoolContext : DbContext
	{
		public DbSet<Course> Courses { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }
		public DbSet<Person> People { get; set; }
		public DbSet<Instructor> Instructors { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

			// fluent API configures many-to-many join table
			modelBuilder.Entity<Course>()
				.HasMany(c => c.Instructors).WithMany(i => i.Courses)
				.Map(t => t.MapLeftKey("CourseID")
					.MapRightKey("InstructorID")
					.ToTable("CourseInstructor"));
			// instruct EF to use sprocs for Insert, Update, and Delete operations
			modelBuilder.Entity<Department>().MapToStoredProcedures();
		}
	}
}