namespace MatchIt.Models
{
    public class Tutor : Student
    {
        public bool IsVolunteer { get; set; }
        public List<MatchingStudents>? MatchingTutees { get; set; }

    }
}
