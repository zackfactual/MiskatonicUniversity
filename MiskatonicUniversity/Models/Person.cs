using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiskatonicUniversity.Models
{
	public abstract class Person
	{
		public int ID { get; set; }

		[Required]
		[StringLength(100)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
		[Column("FirstName")]
		[Display(Name = "First Name")]
		public string FirstMidName { get; set; }

		[Display(Name = "Full Name")]
		public string FullName
		{
			get
			{
				return LastName + ", " + FirstMidName;
			}
		}
	}
}