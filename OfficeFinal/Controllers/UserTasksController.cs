using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeFinal.Models;

namespace OfficeFinal.Controllers
{
    [Authorize]
    public class UserTasksController : Controller
    {
        private DBContext db = new DBContext();

        // GET: UserTasks
        public ActionResult Index()
        {
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();

            var userTasks = db.UserTasks.Where(d => d.CreatorId.Equals(myId)).Include(u => u.AspNetUser).Include(u => u.Status);
            return View(userTasks.ToList());
        }

        // GET: UserTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTask userTask = db.UserTasks.Find(id);
            if (userTask == null)
            {
                return HttpNotFound();
            }
            return View(userTask);
        }

        // GET: UserTasks/Create
        public ActionResult Create()
        {
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();
            ViewBag.CreatorId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(myId)), "Id", "Email");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name");
            ViewBag.TaskTypeId = new SelectList(db.TaskTypes, "Id", "Type");
            return View();
        }

        // POST: UserTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "TaskId,CreatorId,Message,CreateDate,DeadLine,StatusId,files,TaskTypeId")] UserTask userTask, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {


                userTask.files = UploadFile(files);
                db.UserTasks.Add(userTask);
                db.SaveChanges();
                int createdTaskId = db.UserTasks.Where(d => d.Message.Equals(userTask.Message) && d.DeadLine == userTask.DeadLine).Select(d => d.TaskId).FirstOrDefault();
                return RedirectToAction("addPeople", "UserTaskRelations", new { id = createdTaskId });
            }

            ViewBag.CreatorId = new SelectList(db.AspNetUsers, "Id", "Email", userTask.CreatorId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userTask.StatusId);
            return View(userTask);
        }
        //Upload Image
        public string UploadFile(HttpPostedFileBase file)
        {
            string path = "-1";
           
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    //var ext = Path.GetExtension(file.FileName);
                    string filename = Path.GetFileName(file.FileName);
                    //string name = Path.GetFileNameWithoutExtension(filename);
                    //string myfile = name + "_" + DateTime.Now + ext;

                    

                    string filepath = Path.Combine(Server.MapPath("~/Content/UploadedFiles/"), filename);
                    file.SaveAs(filepath);
                    path = "~/Content/UploadedFiles/" + filename;
                }
                catch (Exception ex)
                {
                    path = "-1";
                }

            }


            return path;
        }
        // GET: UserTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTask userTask = db.UserTasks.Find(id);
            if (userTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatorId = new SelectList(db.AspNetUsers, "Id", "Email", userTask.CreatorId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userTask.StatusId);
            return View(userTask);
        }

        // POST: UserTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,CreatorId,Message,CreateDate,DeadLine,StatusId")] UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatorId = new SelectList(db.AspNetUsers, "Id", "Email", userTask.CreatorId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userTask.StatusId);
            return View(userTask);
        }

        // GET: UserTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTask userTask = db.UserTasks.Find(id);
            if (userTask == null)
            {
                return HttpNotFound();
            }
            return View(userTask);
        }

        // POST: UserTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTask userTask = db.UserTasks.Find(id);
            db.UserTasks.Remove(userTask);
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
