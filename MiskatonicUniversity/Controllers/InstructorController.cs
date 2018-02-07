using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MiskatonicUniversity.DAL;
using MiskatonicUniversity.Models;
using MiskatonicUniversity.ViewModels;
using System.Data.Entity.Infrastructure;

namespace MiskatonicUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Instructor
		// load additional related data and put it in the view model
        public ActionResult Index(int? id, int? courseID)
        {
			var viewModel = new InstructorIndexData(); // instantiate view model
			viewModel.Instructors = db.Instructors // put list of instructors in view model
				.Include(i => i.OfficeAssignment) //  specify eager loading for Instructor.OfficeAssignment navigation property
				.Include(i => i.Courses.Select(c => c.Department)) //  specify eager loading for Instructor.Courses navigation property
				.OrderBy(i => i.LastName);

			if (id != null)
			{
				ViewBag.InstructorID = id.Value;
				viewModel.Courses = viewModel.Instructors.Where(
					i => i.ID == id.Value).Single().Courses;
			}

			if (courseID != null)
			{
				/* // lazy loading
				ViewBag.CourseID = courseID.Value;
				viewModel.Enrollments = viewModel.Courses.Where(
					x => x.CourseID == courseID).Single().Enrollments;
				*/

				// explicit loading
				var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
				db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();
				foreach (Enrollment enrollment in selectedCourse.Enrollments)
				{
					db.Entry(enrollment).Reference(x => x.Student).Load();
				}
				viewModel.Enrollments = selectedCourse.Enrollments;
			}
			return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
			var instructor = new Instructor();
			instructor.Courses = new List<Course>();
			PopulateAssignedCourseData(instructor); // provide empty collection for foreach loop in view to avoid null reference exception
            return View();
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
			if (selectedCourses != null)
			{
				// initialize Courses navigation property as an empty collection
				instructor.Courses = new List<Course>();
				// add each selected course to Courses navigation property
				foreach (var course in selectedCourses)
				{
					var courseToAdd = db.Courses.Find(int.Parse(course));
					instructor.Courses.Add(courseToAdd);
				}
			}
			// check for validation errors
            if (ModelState.IsValid)
            {
				// add instructor to dB
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
			PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Instructor instructor = db.Instructors 
				.Include(i => i.OfficeAssignment) // add eager loading for OfficeAssignment entity
				.Include(i => i.Courses) // add eager loading for Courses navigation property
				.Where(i => i.ID == id)
				.Single();
			PopulateAssignedCourseData(instructor); // provide info for checkbox array using AssignedCourseData view model class
            if (instructor == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.ID);
            return View(instructor);
		}

		// provide info for checkbox array using AssignedCourseData view model class
		private void PopulateAssignedCourseData(Instructor instructor)
		{
			var allCourses = db.Courses;
			var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
			var viewModel = new List<AssignedCourseData>();
			foreach (var course in allCourses)
			{
				viewModel.Add(new AssignedCourseData
				{
					CourseID = course.CourseID,
					Title = course.Title,
					Assigned = instructorCourses.Contains(course.CourseID)
				});
			}
			ViewBag.Courses = viewModel;
		}

        // POST: Instructor/Edit/5
        [HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int? id, string[] selectedCourses)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var instructorToUpdate = db.Instructors
				.Include(i => i.OfficeAssignment)
				.Include(i => i.Courses)
				.Where(i => i.ID == id)
				.Single();

			if (TryUpdateModel(instructorToUpdate, "", new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
			{
				try
				{
					if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
					{
						instructorToUpdate.OfficeAssignment = null;
					}
					UpdateInstructorCourses(selectedCourses, instructorToUpdate); // update Courses navigation property of Instructor entity
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				catch (RetryLimitExceededException /* dex */)
				{
					// log the error (uncomment dex variable name and add a line here to write a log)
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				}
			}
			PopulateAssignedCourseData(instructorToUpdate);
			return View(instructorToUpdate);
		}

		// update Courses navigation property of Instructor entity
		private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
		{
			// if no checkboxes selected, initialize Courses navigation property with an empty collection
			if (selectedCourses == null)
			{
				instructorToUpdate.Courses = new List<Course>();
				return;
			}

			var selectedCoursesHS = new HashSet<string>(selectedCourses);
			var instructorCourses = new HashSet<int>
				(instructorToUpdate.Courses.Select(c => c.CourseID));
			foreach (var course in db.Courses)
			{
				// if a checkbox is selected but course isn't in Instructor.Courses navigation property,
					// add the course to the collection in the navigation property
				if (selectedCoursesHS.Contains(course.CourseID.ToString()))
				{
					if (!instructorCourses.Contains(course.CourseID))
					{
						instructorToUpdate.Courses.Add(course);
					}
				}
				// if a checkbox isn't selected but course is in Instructor.Courses navigation property,
					// remove course from navigation property
				else
				{
					if (instructorCourses.Contains(course.CourseID))
					{
						instructorToUpdate.Courses.Remove(course);
					}
				}
			}
		}

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors
				.Include(i => i.OfficeAssignment)
				.Where(i => i.ID == id)
				.Single();

            db.Instructors.Remove(instructor);

			// if instructor is assigned as administrator of any department,
				// remove instructor assignment from that department
					// to avoid referential integrity error in case you delete an administrator instructor
			var department = db.Departments
				.Where(d => d.InstructorID == id)
				.SingleOrDefault();
			if (department != null)
			{
				department.InstructorID = null;
			}

			db.SaveChanges();
            return RedirectToAction("Index");
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
