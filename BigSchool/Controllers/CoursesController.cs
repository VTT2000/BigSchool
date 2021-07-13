using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            // get list Category
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objcourse)
        {
            BigSchoolContext context = new BigSchoolContext();

            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objcourse.ListCategory = context.Categories.ToList();
                return View("Create", objcourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objcourse.LectureId = user.Id;
            
            context.Courses.Add(objcourse);
            context.SaveChanges();
   
            return RedirectToAction("Index","Home");
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach(Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objCourse.LectureId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }

        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Where(c => c.LectureId == currentUser.Id && c.Datetime > DateTime.Now).ToList();
            foreach(Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }

            return View(courses);
        }

        

        public ActionResult DeleteMine(int id)
        {
            BigSchoolContext context = new BigSchoolContext();

            Course deletedCourse = context.Courses.FirstOrDefault(p => p.Id == id);

            return View(deletedCourse);
        }
        [HttpPost]
        public ActionResult DeleteMine(Course x)
        {
            BigSchoolContext context = new BigSchoolContext();

            Attendance deletedAttendance = context.Attendances.FirstOrDefault(p => p.CourseId == x.Id);
            if (deletedAttendance != null)
            {
                context.Attendances.Remove(deletedAttendance);
                context.SaveChanges();
            }

            Course deletedCourse = context.Courses.FirstOrDefault(p => p.Id == x.Id);
            if (deletedCourse != null)
            {
                context.Courses.Remove(deletedCourse);
                context.SaveChanges();
            }

            return RedirectToAction("Mine", "Courses");
        }

        public ActionResult EditMine(int id)
        {
            
            BigSchoolContext context = new BigSchoolContext();
            Course editCourse = context.Courses.FirstOrDefault(p=>p.Id == id);

            if (editCourse != null)
            {
                editCourse.ListCategory = context.Categories.ToList();
            }
            return View(editCourse);
        }
        [HttpPost]
        public ActionResult EditMine([Bind(Include = "Id, Place, Datetime, CategoryId")]Course x)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course editCourse = context.Courses.FirstOrDefault(p=>p.Id == x.Id);
            if(editCourse != null)
            {
                editCourse.Place = x.Place;
                editCourse.Datetime = x.Datetime;
                editCourse.CategoryId = x.CategoryId;
                context.SaveChanges();
            }
            return RedirectToAction("Mine", "Courses");
        }


    }
}