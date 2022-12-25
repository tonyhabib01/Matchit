namespace MatchIt.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Student Student { get; set; }

        public Dictionary<string, string> DayTrimmed { get; }

        public Availability()
        {
            DayTrimmed = new Dictionary<string, string> { 
                { "Monday", "M" },
                { "Tuesday", "T" },
                { "Wednesday", "W" },
                { "Thursday", "Th" },
                { "Friday", "F" },
                { "Saturday", "Sat" },
                { "Sunday", "Sun" }
            };
        }
    }
}
