using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MiskatonicUniversity.DAL;
using MiskatonicUniversity.Models;
using System.Data.Entity.Infrastructure;

namespace MiskatonicUniversity.Controllers
{
    public class CourseController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Course
		// receive and filter by selected value of dropdown list, sort by CourseID, eager load Department navigatino property
        public ActionResult Index(int? SelectedDepartment)
        {
			var departments = db.Departments.OrderBy(q => q.Name).ToList();
			ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
			int departmentID = SelectedDepartment.GetValueOrDefault(0);

			IQueryable<Course> courses = db.Courses
				.Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID) 
				.OrderBy(d => d.CourseID)
				.Include(d => d.Department);
			//var sql = courses.ToString();
            return View(courses.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
			PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
			try
			{
				if (ModelState.IsValid)
				{
					db.Courses.Add(course);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException /* dex */)
			{
				// log the rror (uncomment dex variable name and add line here to write a log
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
			}
			PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
			PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var courseToUpdate = db.Courses.Find(id);
			if (TryUpdateModel(courseToUpdate, "", new string[] { "Title", "Credits", "DepartmentID" }))
			{
				try
				{
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				catch (RetryLimitExceededException /* dex */)
				{
					// log the rror (uncomment dex variable name and add line here to write a log
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				}
			}
			PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
			return View(courseToUpdate);
        }

		private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
		{
			var departmentsQuery = from d in db.Departments
								   orderby d.Name
								   select d;
			ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
		}

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		public ActionResult UpdateCourseCredits()
		{
			return View();
		}

		[HttpPost]
		public ActionResult UpdateCourseCredits(int? multiplier)
		{
			if (multiplier != null)
			{
				ViewBag.RowsAffected = db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
			}
			return View();
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
