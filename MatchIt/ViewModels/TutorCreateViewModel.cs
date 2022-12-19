using MatchIt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MatchIt.ViewModels
{
	public class TutorCreateViewModel
	{
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string? LastName { get; set; }

		[Display(Name = "Student Id")]
		public string? StudentId { get; set; }

		[Display(Name = "Phone Number")]
		public string? PhoneNumber { get; set; }

		[Display(Name = "Email Address")]
		public string? EmailAddress { get; set; }
        public List<SelectListItem> CoursesSelectList { get; set; }
        public List<string> SelectedCourses { get; set; }
        public List<AvailabilityViewModel> Availabilities { get; set; }

		[Display(Name = "Volunteer")]
		public bool IsVolunteer { get; set; }
        public TutorCreateViewModel()
        {
            this.Availabilities = new List<AvailabilityViewModel>();
        }
    }
}
