namespace MatchIt.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Student Student { get; set; }
    }
}
