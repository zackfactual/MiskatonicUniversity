using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiskatonicUniversity.Models
{
	public class OfficeAssignment
	{
		[Key]
		[ForeignKey("Instructor")]
		public int InstructorID { get; set; }

		[StringLength(100)]
		[Display(Name = "Office Location")]
		public string Location { get; set; }

		public virtual Instructor Instructor { get; set; }
	}
}