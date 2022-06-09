using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NguyenVietHoang_BigSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenVietHoang_BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();

            // Không xét valid LecturerId vì bằng user đăng nhập
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }

            //lay login  user tu id
            ApplicationUser user =
                System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            // add vao csdl
            context.Courses.Add(objCourse);
            context.SaveChanges();

            //tro ve home Action index
            return RedirectToAction("Index", "Home");

            
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendancces = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendancces)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }

        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());


            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.Datetime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }

        public void setViewBag(int? selectedId = null)
        {
            var model = new Course();
            ViewBag.CategoryId = new SelectList(model.ListAll(), "Id", "Name", selectedId);
        }

        public ActionResult EdiltMine(int Id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse = context.Courses.Find(Id);
            objCourse.ListCategory = context.Categories.ToList();

            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditMine(Course model)
        {
            BigSchoolContext context = new BigSchoolContext();
            setViewBag(model.CategoryId);
            var updateCourses = context.Courses.Find(model.Id);
            updateCourses.Datetime = model.Datetime;
            updateCourses.LectureName = model.LectureName;
            updateCourses.CategoryId = model.CategoryId;
            var id = context.SaveChanges();
            if (id > 0)
                return RedirectToAction("Mine");
            else
            {
                ModelState.AddModelError("", "Can't save to database");
                return View(model);
            }
        }


        public ActionResult DeleteMine(int Id)
        {
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Find(Id);
            context.Courses.Remove(courses);
            context.SaveChanges();
            return RedirectToAction("Mine");
            
        }
    }
}