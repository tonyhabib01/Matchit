using MatchIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MatchIt.ViewModels
{
	public class TutorCreateViewModel
	{
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? StudentId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public List<SelectListItem> CoursesSelectList { get; set; }
        public List<string> SelectedCourses { get; set; }
        public List<AvailabilityViewModel> Availabilities { get; set; }
        public bool IsVolunteer { get; set; }
        public TutorCreateViewModel()
        {
            this.Availabilities = new List<AvailabilityViewModel>();
        }
    }
}
