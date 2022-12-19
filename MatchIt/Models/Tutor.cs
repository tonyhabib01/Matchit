using System.ComponentModel.DataAnnotations;

namespace MatchIt.Models
{
    public class Tutor : Student
    {
		[Display(Name = "Volunteer")]
		public bool IsVolunteer { get; set; }
        public List<MatchingStudents>? MatchingTutees { get; set; }

    }
}
