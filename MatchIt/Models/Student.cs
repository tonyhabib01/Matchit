using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MatchIt.Models
{
    public class Student
    {
        public int Id { get; set; }

		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Display(Name = "Student Id")]
		public string StudentId { get; set; }

		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }

		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }
		public Semester Semester { get; set; }
        public List<Course> Courses { get; set; }
        public List<Availability> Availabilities { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
