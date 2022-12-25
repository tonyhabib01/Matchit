using MatchIt.Data;
using MatchIt.Models;
using MatchIt.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;

namespace MatchIt.Controllers
{
    public class TuteeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public TuteeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
		}


        // GET: TuteeController
        public ActionResult List()
        {
			ViewBag.Page = "Tutees";
            try
            {
			    var semester = _context.Semesters.OrderByDescending(s => s.Id).First();
                return View(_context.Tutees.Include(t => t.Courses).Where(t => t.Semester == semester));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Couldn't fetch tutees data.";
                return View(new List<Tutee>());
            }
        }

        // GET: TuteeController/Details/5
        public ActionResult Details(int id)
        {
			ViewBag.Page = "Tutees";
			return View();
        }

        // GET: TuteeController/Create
        public ActionResult Create()
        {
			ViewBag.Page = "Tutees";
			ViewData["Courses"] = _context.Courses;
            var vModel = new TuteeCreateViewModel();
            vModel.CoursesSelectList = new List<SelectListItem>();

            foreach(var course in _context.Courses)
            {
                vModel.CoursesSelectList.Add(new SelectListItem { Text = course.Code, Value = course.Id.ToString() });
            }
            return View(vModel);
        }

        // POST: TuteeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TuteeCreateViewModel tuteeViewModel)
        {
            try
            {
                var availabilities = new List<Availability>();
                foreach (var availability in tuteeViewModel.Availabilities)
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
                var courses = _context.Courses.Where(c => tuteeViewModel.SelectedCourses.Contains(c.Id.ToString()));
                var tutee = new Tutee
                {
                    FirstName = tuteeViewModel.FirstName,
                    LastName = tuteeViewModel.LastName,
                    StudentId = tuteeViewModel.StudentId,
                    PhoneNumber = tuteeViewModel.PhoneNumber,
                    EmailAddress = tuteeViewModel.EmailAddress,
                    Semester = semester,
                    Courses = courses.ToList(),
                    Availabilities = availabilities
                };
                _context.Add(tutee);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tutee created successfully.";
                return RedirectToAction(nameof(List));

            }
            catch( Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to create a new tutee.";
                return RedirectToAction(nameof(List));
            }
        }

        // GET: TuteeController/Edit/5
        public ActionResult Edit(int id)
        {
			ViewBag.Page = "Tutees";
			var tutee = _context.Tutees.Include(t => t.Availabilities).Include(t => t.Courses).SingleOrDefault(t => t.Id == id); // Eager Loading
            if (tutee == null)
            {
                TempData["ErrorMessage"] = "Invalid tutee id.";
                return RedirectToAction(nameof(List));
            }

            var vModel = new TuteeCreateViewModel
            {
                FirstName = tutee.FirstName,
                LastName = tutee.LastName,
                EmailAddress = tutee.EmailAddress,
                PhoneNumber = tutee.PhoneNumber,
                StudentId = tutee.StudentId,

            };

            foreach (var availability in tutee.Availabilities)
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

            List<string> selectedCourses = tutee.Courses.Select(c => c.Id.ToString()).ToList();
            vModel.SelectedCourses = selectedCourses;

            return View(vModel);
        }

        // POST: TuteeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TuteeCreateViewModel tuteeViewModel)
        {

            var tutee = _context.Tutees.Include(t => t.Availabilities).Include(t => t.Courses).SingleOrDefault(t => t.Id == id); // Eager loading

            var availabilities = new List<Availability>();
            foreach (var availability in tuteeViewModel.Availabilities)
            {
                var av = new Availability
                {
                    Day = availability.Day,
                    From = DateTime.ParseExact(availability.From, "HH:mm", CultureInfo.InvariantCulture),
                    To = DateTime.ParseExact(availability.To, "HH:mm", CultureInfo.InvariantCulture),
                };
                av.From = new DateTime(1970, 1, 1 , av.From.Hour, av.From.Minute, av.From.Second);
                av.To = new DateTime(1970, 1, 1, av.To.Hour, av.To.Minute, av.To.Second);
                availabilities.Add(av);
            }
            var courses = _context.Courses.Where(c => tuteeViewModel.SelectedCourses.Contains(c.Id.ToString()));
            tutee.FirstName = tuteeViewModel.FirstName;
            tutee.LastName = tuteeViewModel.LastName;
            tutee.StudentId = tuteeViewModel.StudentId;
            tutee.PhoneNumber = tuteeViewModel.PhoneNumber;
            tutee.EmailAddress = tuteeViewModel.EmailAddress;
            tutee.Courses.Clear();
            tutee.Courses = courses.ToList();
            tutee.Availabilities.Clear();
            tutee.Availabilities = availabilities;

            _context.Update(tutee);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tutee edited successfully.";
            return RedirectToAction(nameof(List));
        }

        // GET: TuteeController/Delete/5
        public ActionResult Delete(int id)
        {
			ViewBag.Page = "Tutees";
			var tutee = _context.Tutees.SingleOrDefault(t => t.Id == id);
            if (tutee == null)
            {
                TempData["ErrorMessage"] = "Invalid tutee id.";
                return RedirectToAction(nameof(List));
            }

            return View(tutee);
        }

        // POST: TuteeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tutee tutee)
        {
            if (id != tutee.Id)
            {
                TempData["ErrorMessage"] = "Invalid tutee Id.";
                return RedirectToAction(nameof(List));
            }

            try
            {
                var tut = _context.Tutees.Include(t => t.Semester).Single(t => t.Id == id);
                var matchedList = _context.MatchingStudents
                    .Include(m => m.Tutor)
                    .Include(m => m.Tutee)
                    .Include(m => m.Course)
                    .Where(m => m.Tutee.Semester.Id == tut.Semester.Id);
                _context.RemoveRange(matchedList);
                _context.Remove(tut);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tutee deleted successfully.";
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to delete tutee.";
                return RedirectToAction(nameof(List));
            }
        }
    }
}
