using ClosedXML.Excel;
using MatchIt.Data;
using MatchIt.DTO;
using MatchIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace MatchIt.Controllers
{
    [Authorize]
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
            try
            {
                var semesters = _context.Semesters.OrderByDescending(s => s.Id);
                return View(semesters);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Couldn't fetch semesters data.";
                return View(new List<Semester>());
            }
        }

        // GET: SemesterController/Create
        public ActionResult Create()
        {
			ViewBag.Page = "Semesters";
			return View();
        }

        // POST: SemesterController/Create
        // localhost:port /Controller / create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Semester semester)
        {
            try
            {
                _context.Add(semester);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Semester created successfully.";
                return RedirectToAction(nameof(List));

            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to create a new semester.";
                return RedirectToAction(nameof(List));

            }
        }

        // GET: SemesterController/Edit/5
        public ActionResult Edit( int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
            {
                TempData["ErrorMessage"] = "Invalid semester id.";
                return RedirectToAction(nameof(List));
            }

            return View(semester);
        }

        // POST: SemesterController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
                
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(semester);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Semester edited successfully.";
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to edit semester.";
                    return RedirectToAction(nameof(Edit), new { id });

                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to edit semester.";
                return RedirectToAction(nameof(List));
            }
        }

        // GET: SemesterController/Delete/5
        public ActionResult Delete(int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
            }

            return View(semester);
        }

        // POST: SemesterController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Semester semester)
        {
            if (id != semester.Id)
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
            }

            try
            {
                var matchedList = _context.MatchingStudents
                    .Include(m => m.Tutor)
                    .Include(m => m.Tutee)
                    .Include(m => m.Course)
                    .Where(m => m.Tutor.Semester.Id == semester.Id);
               
                _context.RemoveRange(matchedList);
                _context.Remove(semester);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Semester deleted successfully.";
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to delete semester.";
                return RedirectToAction(nameof(List));
            }
        }

        public ActionResult MatchedList(int id)
        {
			ViewBag.Page = "Semesters";
			var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
            }
            var matchedList = _context.MatchingStudents
                .Include(m => m.Tutor)
                .Include(m => m.Tutor.Availabilities)
                .Include(m => m.Tutee)
                .Include(m => m.Tutee.Availabilities)
                .Include(m => m.Course)
                .Where(m => m.Tutor.Semester.Id == semester.Id);

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
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
            }
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
                            TempData["ErrorMessage"] = "Unable to match students please check if the tutees and tutors data are correct.";
                            objTrans.Rollback();
                        }
                    }
                }
               
                if (con.State == ConnectionState.Open)
                    con.Close();

                return RedirectToAction(nameof(MatchedList), new { id });
              
            }
        }


        public ActionResult ExportList(int id)
        {
            var semester = _context.Semesters.SingleOrDefault(sem => sem.Id == id);
            if (semester == null)
            {
                TempData["ErrorMessage"] = "Invalid semester Id.";
                return RedirectToAction(nameof(List));
            }
            var matchedList = _context.MatchingStudents
                .Include(m => m.Tutor)
                .Include(m => m.Tutor.Availabilities)
                .Include(m => m.Tutee)
                .Include(m => m.Tutee.Availabilities)
                .Include(m => m.Course)
                .Where(m => m.Tutor.Semester.Id == semester.Id).ToList();

            // Create a new workbook
            var workbook = new XLWorkbook();

            // Add a worksheet
            var worksheet = workbook.Worksheets.Add("MatchedList");
            worksheet.Style.Font.SetFontSize(12);
            worksheet.Style.Font.SetFontName("Times New Roman");
            // Set the header row
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 1).Style.Font.SetBold(true);
            worksheet.Cell(1, 1).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 2).Value = "Tutor";
            worksheet.Cell(1, 2).Style.Font.SetBold(true);
            worksheet.Cell(1, 2).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 3).Value = "Availabilities";
            worksheet.Cell(1, 3).Style.Font.SetBold(true);
            worksheet.Cell(1, 3).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 4).Style.Font.SetBold(true);
            worksheet.Cell(1, 4).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 5).Style.Font.SetBold(true);
            worksheet.Cell(1, 5).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 6).Value = "Id";
            worksheet.Cell(1, 6).Style.Font.SetBold(true);
            worksheet.Cell(1, 6).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 7).Value = "Tutee";
            worksheet.Cell(1, 7).Style.Font.SetBold(true);
            worksheet.Cell(1, 7).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 8).Value = "Availabilities";
            worksheet.Cell(1, 8).Style.Font.SetBold(true);
            worksheet.Cell(1, 8).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 9).Value = "Email";
            worksheet.Cell(1, 9).Style.Font.SetBold(true);
            worksheet.Cell(1, 9).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 10).Value = "Phone";
            worksheet.Cell(1, 10).Style.Font.SetBold(true);
            worksheet.Cell(1, 10).Style.Font.SetFontSize(14);
            worksheet.Cell(1, 11).Value = "Course";
            worksheet.Cell(1, 11).Style.Font.SetBold(true);
            worksheet.Cell(1, 11).Style.Font.SetFontSize(14);
            
            // Populate the data rows
            for (int i = 0; i < matchedList.Count; i++)
            {
                var row = matchedList[i];
                //Tutor
                worksheet.Cell(i + 2, 1).Value = row.Tutor.StudentId;
                worksheet.Cell(i + 2, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 2).Value = row.Tutor.ToString();
                worksheet.Cell(i + 2, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var tutorAvCell = worksheet.Cell(i + 2, 3);
                var richText = tutorAvCell.CreateRichText();
                for(int j = 0; j < row.Tutor.Availabilities.Count; j++)
                {
                    var avTutor = row.Tutor.Availabilities[j];
                    richText.AddText(avTutor.DayTrimmed[avTutor.Day] + ": " + avTutor.From.ToShortTimeString() + " - " + avTutor.To.ToShortTimeString());
                    if (j != row.Tutor.Availabilities.Count - 1)
                        richText.AddNewLine();
                }
                worksheet.Cell(i + 2, 3).Value = richText.ToString();
                worksheet.Cell(i + 2, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 4).Value = row.Tutor.EmailAddress;
                worksheet.Cell(i + 2, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 5).Value = row.Tutor.PhoneNumber;
                worksheet.Cell(i + 2, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Tutee
                worksheet.Cell(i + 2, 6).Value = row.Tutee.StudentId;
                worksheet.Cell(i + 2, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 7).Value = row.Tutee.ToString();
                worksheet.Cell(i + 2, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var tuteeAvCell = worksheet.Cell(i + 2, 8);
                richText = tuteeAvCell.CreateRichText();
                for (int j = 0; j < row.Tutee.Availabilities.Count; j++)
                {
                    var avTutee = row.Tutee.Availabilities[j];
                    richText.AddText(avTutee.DayTrimmed[avTutee.Day] + ": " + avTutee.From.ToShortTimeString() + " - " + avTutee.To.ToShortTimeString());
                    if (j != row.Tutee.Availabilities.Count - 1)
                        richText.AddNewLine();
                }

                worksheet.Cell(i + 2, 8).Value = richText.ToString();
                worksheet.Cell(i + 2, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(i + 2, 9).Value = row.Tutee.EmailAddress;
                worksheet.Cell(i + 2, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 10).Value = row.Tutee.PhoneNumber;
                worksheet.Cell(i + 2, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(i + 2, 11).Value = row.Course.ToString();
                worksheet.Cell(i + 2, 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            }

            //Adjust Rows and Columns width and height
            worksheet.Rows().AdjustToContents();
            worksheet.Columns().AdjustToContents();
            // Save the workbook to a memory stream
            var stream = new MemoryStream();
            workbook.SaveAs(stream);

            // Set the stream position to the beginning
            stream.Position = 0;

            // Return the file as a download
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "matched_list.xlsx");


        }

    }
}

