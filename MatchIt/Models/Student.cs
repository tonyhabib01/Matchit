namespace MatchIt.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string PhoneNumber { get; set; }
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
