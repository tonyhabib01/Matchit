using MatchIt.Data;
using MatchIt.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchIt.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public CourseController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: CourseController
        public ActionResult List()
        {
			ViewBag.Page = "Courses";
            try
            {
                return View(_context.Courses);
            }
            catch
            {
                TempData["ErrorMessage"] = "Couldn't fetch courses data.";
                return View(new List<Semester>());
            }
        }

        // GET: CourseController/Create
        public ActionResult Create()
        {
			ViewBag.Page = "Courses";
			return View();
        }

        // POST: CourseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course course)
        {
            try
            {
                course.Code = course.Code.Replace(" ", String.Empty);
                _context.Add(course);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Course created successfully.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to create a new course.";
                return RedirectToAction(nameof(List));
            }
        }

        // GET: CourseController/Edit/5
        public ActionResult Edit(int id)
        {
			ViewBag.Page = "Courses";
			var course = _context.Courses.SingleOrDefault(c => c.Id == id);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Invalid course id.";
                return RedirectToAction(nameof(List));
            }

            return View(course);
        }

        // POST: CourseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                TempData["ErrorMessage"] = "Invalid course Id.";
                return RedirectToAction(nameof(List));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(course);
                    _context.SaveChanges();
                }
                TempData["SuccessMessage"] = "Course edited successfully.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to edit course.";
                return RedirectToAction(nameof(List));
            }
        }

        // GET: CourseController/Delete/5
        public ActionResult Delete(int id)
        {
			ViewBag.Page = "Courses";
			var course = _context.Courses.SingleOrDefault(c => c.Id == id);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Invalid course Id.";
                return RedirectToAction(nameof(List));
            }

            return View(course);
        }

        // POST: CourseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Course course)
        {
            if (id != course.Id)
            {
                TempData["ErrorMessage"] = "Invalid course Id.";
                return RedirectToAction(nameof(List));
            }

            try
            {

                _context.Remove(course);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Course deleted successfully.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to delete course.";
                return RedirectToAction(nameof(List));
            }
        }
    }
}
