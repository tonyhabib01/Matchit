using MatchIt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchIt.Controllers
{
    [Authorize]
    public class MatchingStudentsController : Controller
	{

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public MatchingStudentsController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: MatchingStudentsController
        public ActionResult Index()
		{
			return View();
		}

		// GET: MatchingStudentsController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: MatchingStudentsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: MatchingStudentsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: MatchingStudentsController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: MatchingStudentsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: MatchingStudentsController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: MatchingStudentsController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
