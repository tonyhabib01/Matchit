using MatchIt.Data;
using MatchIt.Models;
using MatchIt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MatchIt.Controllers
{
    [Authorize]
    public class TutorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public TutorController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: TutorController
        public ActionResult List()
        {
			ViewBag.Page = "Tutors";
            try
            {
			    var semester = _context.Semesters.OrderByDescending(s => s.Id).First();
                return View(_context.Tutors.Include(t => t.Courses).Include(t => t.Availabilities).Where(t => t.Semester == semester));
            } 
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Couldn't fetch tutors data.";
                return View(new List<Tutor>());
            }
        }

        // GET: TutorController/Details/5
        public ActionResult Details(int id)
        {
			ViewBag.Page = "Tutors";
			return View();
        }

        // GET: TutorController/Create
        public ActionResult Create()
        {
			ViewBag.Page = "Tutors";
			ViewData["Courses"] = _context.Courses;
            var vModel = new TutorCreateViewModel();
            vModel.CoursesSelectList = new List<SelectListItem>();
            foreach (var course in _context.Courses)
            {
                vModel.CoursesSelectList.Add(new SelectListItem { Text = course.Code, Value = course.Id.ToString() });
            }
            return View(vModel);
        }

        // POST: TutorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TutorCreateViewModel tutorViewModel)
        {
            try
            {
                var availabilities = new List<Availability>();
                foreach (var availability in tutorViewModel.Availabilities)
                {
                    var av = new Availability
                    {
                        Day = availability.Day,
                        From = DateTime.ParseExact(availability.From, "HH:mm", CultureInfo.InvariantCulture),
                        To = DateTime.ParseExact(availability.To, "HH:mm", CultureInfo.InvariantCulture),
                    };
                    av.From = new DateTime(1970, 1, 1, av.From.Hour, av.From.Minute, av.From.Second);
                    av.To = new DateTime(1970, 1, 1, av.To.Hour, av.To.Minute, av.To.Second);
                    availabilities.Add(av);
                }
                var semester = _context.Semesters.OrderByDescending(s => s.Id).First();
                var courses = _context.Courses.Where(c => tutorViewModel.SelectedCourses.Contains(c.Id.ToString()));
                var tutor = new Tutor
                {
                    FirstName = tutorViewModel.FirstName,
                    LastName = tutorViewModel.LastName,
                    StudentId = tutorViewModel.StudentId,
                    PhoneNumber = tutorViewModel.PhoneNumber,
                    EmailAddress = tutorViewModel.EmailAddress,
                    IsVolunteer = tutorViewModel.IsVolunteer,
                    Semester = semester,
                    Courses = courses.ToList(),
                    Availabilities = availabilities
                };
                _context.Add(tutor);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tutor created successfully.";
                return RedirectToAction(nameof(List));

            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to create a new tutor.";
                return RedirectToAction(nameof(List));
            }
        }

        // GET: TutorController/Edit/5
        public ActionResult Edit(int id)
        {
			ViewBag.Page = "Tutors";
			var tutor = _context.Tutors.Include(t => t.Availabilities).Include(t => t.Courses).SingleOrDefault(t => t.Id == id); // Eager Loading
            if (tutor == null)
            {
                TempData["ErrorMessage"] = "Invalid tutee id.";
                return RedirectToAction(nameof(List));
            }


            var vModel = new TutorCreateViewModel
            {
                IsVolunteer = tutor.IsVolunteer,
                FirstName = tutor.FirstName,
                LastName = tutor.LastName,
                EmailAddress = tutor.EmailAddress,
                PhoneNumber = tutor.PhoneNumber,
                StudentId = tutor.StudentId,

            };

            foreach (var availability in tutor.Availabilities)
            {
                var av = new AvailabilityViewModel();
                av.Day = availability.Day;
                av.From = availability.From.ToString("HH:mm");
                av.To = availability.To.ToString("HH:mm");
                vModel.Availabilities.Add(av);
            }

            vModel.CoursesSelectList = new List<SelectListItem>();

            foreach (var course in _context.Courses)
            {
                vModel.CoursesSelectList.Add(new SelectListItem { Text = course.Code, Value = course.Id.ToString() });
            }

            List<string> selectedCourses = tutor.Courses.Select(c => c.Id.ToString()).ToList();
            vModel.SelectedCourses = selectedCourses;

            return View(vModel);
        }

        // POST: TutorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TutorCreateViewModel tutorViewModel)
        {
            var tutor = _context.Tutors.Include(t => t.Availabilities).Include(t => t.Courses).SingleOrDefault(t => t.Id == id); // Eager loading

            var availabilities = new List<Availability>();
            foreach (var availability in tutorViewModel.Availabilities)
            {
                var av = new Availability
                {
                    Day = availability.Day,
                    From = DateTime.ParseExact(availability.From, "HH:mm", CultureInfo.InvariantCulture),
                    To = DateTime.ParseExact(availability.To, "HH:mm", CultureInfo.InvariantCulture),
                };
                av.From = new DateTime(1970, 1, 1, av.From.Hour, av.From.Minute, av.From.Second);
                av.To = new DateTime(1970, 1, 1, av.To.Hour, av.To.Minute, av.To.Second);
                availabilities.Add(av);
            }
            var courses = _context.Courses.Where(c => tutorViewModel.SelectedCourses.Contains(c.Id.ToString()));
            tutor.FirstName = tutorViewModel.FirstName;
            tutor.LastName = tutorViewModel.LastName;
            tutor.StudentId = tutorViewModel.StudentId;
            tutor.PhoneNumber = tutorViewModel.PhoneNumber;
            tutor.EmailAddress = tutorViewModel.EmailAddress;
            tutor.Courses.Clear();
            tutor.Courses = courses.ToList();
            tutor.Availabilities.Clear();
            tutor.Availabilities = availabilities;

            try
            {
                _context.Update(tutor);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tutee edited successfully.";
            }
            catch (Exception ex) 
            {
                TempData["ErrorMessage"] = "Unable to edit tutee.";
            }

            return RedirectToAction(nameof(List));
        }

        // GET: TutorController/Delete/5
        public ActionResult Delete(int id)
        {
			ViewBag.Page = "Tutors";
			var tutor = _context.Tutors.SingleOrDefault(t => t.Id == id);
            if (tutor == null)
            {
                TempData["ErrorMessage"] = "Invalid tutor Id.";
                return RedirectToAction(nameof(List));
            }

            return View(tutor);
        }

        // POST: TutorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tutor tutor)
        {
            if (id != tutor.Id)
            {
                TempData["ErrorMessage"] = "Invalid tutor Id.";
                return RedirectToAction(nameof(List));
            }

            try
            {
                var tut = _context.Tutors.Include(t => t.Semester).Single(t => t.Id == id);
                var matchedList = _context.MatchingStudents
                    .Include(m => m.Tutor)
                    .Include(m => m.Tutee)
                    .Include(m => m.Course)
                    .Where(m => m.Tutor.Semester.Id == tut.Semester.Id);
                _context.RemoveRange(matchedList);
                _context.Remove(tut);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tutor deleted successfully.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to delete tutor.";
                return RedirectToAction(nameof(List));
            }
        }
    }
}
