namespace MatchIt.Models
{
    public class MatchingStudents
    {
        public int Id { get; set; }
        public Tutor? Tutor { get; set; }
        public Tutee? Tutee { get; set; }
        public Course? Course { get; set; }
    }
}
