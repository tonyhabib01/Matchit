using MatchIt.Data;
using MatchIt.DTO;
using MatchIt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace MatchIt.Controllers
{
    public class SemesterController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public SemesterController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: SemesterController
        public ActionResult List()
        {
			ViewBag.Page = "Semesters";
			return View(_context.Semesters);
        }

        // GET: SemesterController/Create
        public ActionResult Create()
        {
			ViewBag.Page = "Semesters";
			return View();
        }

        // POST: SemesterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Semester semester)
        {
            try
            {
                _context.Add(semester);
                _context.SaveChanges();
                return RedirectToAction(nameof(List));

            }
            catch
            {
                return RedirectToAction(nameof(List));

            }
        }

        // GET: SemesterController/Edit/5
        public ActionResult Edit(int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
                return RedirectToAction(nameof(List));

            return View(semester);
        }

        // POST: SemesterController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                return RedirectToAction(nameof(List));
                //return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(semester);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(List));
            }
            catch
            {
                return RedirectToAction(nameof(List));
            }
        }

        // GET: SemesterController/Delete/5
        public ActionResult Delete(int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
                return RedirectToAction(nameof(List));

            return View(semester);
        }

        // POST: SemesterController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                return RedirectToAction(nameof(List));
            }

            try
            {

                _context.Remove(semester);
                _context.SaveChanges();

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return RedirectToAction(nameof(List));
            }
        }

        public ActionResult MatchedList(int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
                return RedirectToAction(nameof(List));
            var matchedList = _context.MatchingStudents
                .Include(m => m.Tutor)
                .Include(m => m.Tutee)
                .Include(m => m.Course);

            return View(matchedList.ToList());
        }

        public ActionResult MatchStudents(int id)
        {
			ViewBag.Page = "Semesters";
			var matchedStudents = _context.MatchingStudents.Include(m => m.Tutor).Include(m => m.Tutor.Semester).Where(m => m.Tutor.Semester.Id == id);
            _context.RemoveRange(matchedStudents);
            _context.SaveChanges();

            var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
                return RedirectToAction(nameof(List));
            var tutorsIds = new List<int>();
            SqlTransaction objTrans = null;
            using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
            {
                con.Open();
                var tutorsQuery = @$"
                    SELECT 
                        s.*
                    FROM 
                        Students AS s
                    INNER JOIN 
                        CourseStudent AS cs ON cs.StudentsId = s.Id
                    WHERE 
                        s.StudentType = 'tutor'
                        AND
                        s.SemesterId = {semester.Id}
                    GROUP BY 
                        s.Id, s.FirstName, s.LastName, 
                        s.StudentId, s.PhoneNumber, 
                        s.EmailAddress, s.SemesterId, s.StudentType, s.IsVolunteer
                    ORDER BY COUNT(*);
                ";
                using (SqlCommand cmd = new SqlCommand(tutorsQuery))
                {
                    cmd.Connection = con;
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            tutorsIds.Add((int)sdr["Id"]);
                        }
                    }
                }

                foreach (var tutorId in tutorsIds)
                {
                    var availabilityQuery = $"SELECT * FROM Availabilities WHERE StudentId = {tutorId}";
                    var availabilitiesArrayQuery = new List<string>();
                    using (SqlCommand cmd = new SqlCommand(availabilityQuery))
                    {
                        cmd.Connection = con;
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                // The following code is injection safe.
                                //availabilitiesArrayQuery.Add($"((a.[From] BETWEEN '{sdr["From"]}' AND '{sdr["To"]}') OR (a.[To] BETWEEN '{sdr["From"]}' AND '{sdr["To"]}') AND (a.[Day] = '{sdr["To"]}'))");
                                availabilitiesArrayQuery.Add($"((a.[From] <= '{sdr["From"]}' AND a.[To] > '{sdr["From"]}') AND (a.[Day] = '{sdr["Day"]}'))");
                                availabilitiesArrayQuery.Add($"((a.[From] < '{sdr["To"]}' AND a.[To] >= '{sdr["To"]}') AND (a.[Day] = '{sdr["Day"]}'))");
                                availabilitiesArrayQuery.Add($"((a.[From] >= '{sdr["From"]}' AND a.[To] <= '{sdr["To"]}') AND (a.[Day] = '{sdr["Day"]}'))");

                            }
                        }
                    }


                    var tuteeQuery = @$"
                        SELECT TOP 2
                         s.id AS StudentId,
                         cs.CoursesId AS CourseId
                        FROM 
                         Students AS s
                        INNER JOIN 
                         CourseStudent AS cs ON cs.StudentsId = s.Id
                        INNER JOIN 
                         Availabilities AS a ON a.StudentId = s.Id
                        WHERE
                         s.StudentType = 'tutee'
                         AND
                         s.SemesterId = {semester.Id}
                         AND 
                          cs.CoursesId IN (
                           SELECT  cs.CoursesId FROM CourseStudent AS cs
                           WHERE cs.StudentsId = {tutorId}
                          )
                         AND 
                            (
                                {string.Join(" OR ", availabilitiesArrayQuery.ToArray())}
                            )
                         AND 
                          (
                           CASE WHEN
                            (SELECT COUNT (*) FROM MatchingStudents WHERE TuteeId = s.Id GROUP BY TuteeId) IS NULL
                           THEN 0
                           ELSE
                            (SELECT COUNT (*) FROM MatchingStudents WHERE TuteeId = s.Id GROUP BY TuteeId)
                           END
                          ) < 2
                        GROUP BY s.id, cs.CoursesId
                        ORDER BY 
                         (COUNT (*) OVER ( PARTITION BY s.Id))
                    ";

                    var matchedTutees = new List<TuteeMatchDTO>();

                    using (SqlCommand cmd = new SqlCommand(tuteeQuery))
                    {
                        cmd.Connection = con;
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                matchedTutees.Add(new TuteeMatchDTO() { StudentId = (int)sdr["StudentId"], CourseId = (int)sdr["CourseId"] });
                            }
                        }
                    }
                    if (matchedTutees.Count > 0)
                    {
                        objTrans = con.BeginTransaction();
                        try
                        {
                            var q = "INSERT INTO MatchingStudents (TutorId, TuteeId, CourseId) VALUES";
                            for (int i = 0; i < matchedTutees.Count; i++)
                            {
                                q += $"({tutorId}, {matchedTutees[i].StudentId}, {matchedTutees[i].CourseId})";
                                if (i != matchedTutees.Count - 1)
                                    q += ", ";
                            }
                            var sqlCommand = new SqlCommand(q, con, objTrans);
                            sqlCommand.ExecuteNonQuery();
                            objTrans.Commit();

                        }
                        catch (Exception)
                        {
                            objTrans.Rollback();
                        }
                        //finally
                        //{
                        //    con.Close();
                        //}
                    }


                }
                // The following statement should never be true, But just in case this action was called without any tutors.
                //This check just in case this case was executed without any tutors. (This should never )
                if (con.State == ConnectionState.Open)
                    con.Close();

                return View();
              
            }
        }
        //public ActionResult MatchStudents(int id)
        //{
        //    var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
        //    if (semester == null)
        //        return RedirectToAction(nameof(List));


        //    //var tutorsAggr = _context.Tutors
        //    //    .Include(t => t.Courses)
        //    //    .Select(t => new {Id = t.Id, CourseCount = t.Courses.Count()})
        //    //    .OrderBy(t => t.CourseCount);
        //    var tutors = _context.Tutors.FromSqlRaw(@"
        //        SELECT s.* FROM Students AS s
        //        INNER JOIN CourseStudent AS cs ON cs.StudentsId = s.Id
        //        WHERE s.StudentType = 'tutor'
        //        GROUP BY 
        //        s.Id, s.FirstName, s.LastName, 
        //        s.StudentId, s.PhoneNumber, 
        //        s.EmailAddress, s.SemesterId, s.StudentType, s.IsVolunteer
        //        ORDER BY COUNT(*);
        //    ").ToList();
        //    //using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString())){

        //    //}
        //    //_context.Database.Connection
        //foreach (var tutor in tutors)
        //{
        //    StringBuilder stringBuilder = new StringBuilder("");
        //    for (int i = 0; i<tutor.Availabilities.Count; i++)
        //    {
        //        if (i == 0) 
        //            stringBuilder.Append(" ( ");

        //        stringBuilder.Append($" ((a.[From] BETWEEN '{tutor.Availabilities[i].From.ToString()}' AND '{tutor.Availabilities[i].To.ToString()}') OR (a.[To] BETWEEN '{tutor.Availabilities[0].From.ToString()}' AND '{tutor.Availabilities[0].To.ToString()}') AND (a.[Day] = '{tutor.Availabilities[0].Day}')) ");
        //        if (i == tutor.Availabilities.Count - 1)
        //        {
        //            stringBuilder.Append(" ) ");
        //        } 
        //        else
        //        {
        //            stringBuilder.Append(" OR ");
        //        }

        //    }

        ////var query = @$"
        ////            SELECT TOP 2
        ////             s.id AS StudentId,
        ////                cs.CoursesId AS CourseId
        ////            FROM 
        ////             Students AS s
        ////            INNER JOIN 
        ////             CourseStudent AS cs ON cs.StudentsId = s.Id
        ////            INNER JOIN 
        ////             Availabilities AS a ON a.StudentId = s.Id
        ////            WHERE
        ////                    s.StudentType = 'tutee'
        ////                AND
        ////              s.SemesterId = {semester.Id}
        ////             AND 
        ////              cs.CoursesId IN (
        ////               SELECT  cs.CoursesId FROM CourseStudent AS cs
        ////               WHERE cs.StudentsId = {tutor.Id}
        ////              )
        ////             AND 
        ////              {stringBuilder}
        ////             AND 
        ////              (
        ////               CASE WHEN
        ////                (SELECT COUNT (*) FROM MatchingStudents WHERE TuteeId = s.Id GROUP BY TuteeId) IS NULL
        ////               THEN 0
        ////               ELSE
        ////                (SELECT COUNT (*) FROM MatchingStudents WHERE TuteeId = s.Id GROUP BY TuteeId)
        ////               END
        ////              ) < 2
        ////            ORDER BY 
        ////             (COUNT (*) OVER ( PARTITION BY s.Id))
        ////        ";

        //        var tutees = _context.Tutees.FromSqlRaw(query).Select(t => new TuteeMatchDTO() { StudentId = t["StudentId"], CourseId = t["CourseId"] });
        //        //foreach (var tut in tutees)
        //        //{
        //        //    var matchingStudents = new MatchingStudents
        //        //    {

        //        //    };
        //        //}
        //    }

        //    return RedirectToAction(nameof(List));
        //}
    }
}
