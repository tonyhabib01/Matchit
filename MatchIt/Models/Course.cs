namespace MatchIt.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Student>? Students { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
