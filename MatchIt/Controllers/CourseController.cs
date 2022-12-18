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
            return View(_context.Courses);
        }

        // GET: CourseController/Create
        public ActionResult Create()
        {
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
                return RedirectToAction(nameof(List));

            }
            catch
            {
                return RedirectToAction(nameof(List));

            }
        }

        // GET: CourseController/Edit/5
        public ActionResult Edit(int id)
        {
            var course = _context.Courses.SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction(nameof(List));

            return View(course);
        }

        // POST: CourseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                return RedirectToAction(nameof(List));
                //return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(course);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(List));
            }
            catch
            {
                return RedirectToAction(nameof(List));
            }
        }

        // GET: CourseController/Delete/5
        public ActionResult Delete(int id)
        {
            var course = _context.Courses.SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction(nameof(List));

            return View(course);
        }

        // POST: CourseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Course course)
        {
            if (id != course.Id)
            {
                return RedirectToAction(nameof(List));
            }

            try
            {

                _context.Remove(course);
                _context.SaveChanges();

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return RedirectToAction(nameof(List));
            }
        }
    }
}
