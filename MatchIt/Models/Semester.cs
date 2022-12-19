using System.ComponentModel.DataAnnotations;

namespace MatchIt.Models
{
    public class Semester
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public List<Student>? Students { get; set; }
    }
}
