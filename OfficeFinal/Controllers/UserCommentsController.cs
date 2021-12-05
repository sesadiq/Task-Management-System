using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OfficeFinal.Models;

namespace OfficeFinal.Controllers
{
    [Authorize]
    public class UserCommentsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: UserComments
        public ActionResult Index()
        {
            var userComments = db.UserComments.Include(u => u.Status).Include(u => u.UserTasks);
            return View(userComments.ToList());
        }

        // GET: UserComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserComment userComment = db.UserComments.Find(id);
            if (userComment == null)
            {
                return HttpNotFound();
            }
            return View(userComment);
        }

        // GET: UserComments/Create
        public ActionResult Create2(int? id)
        {
          
            var AuthUserId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();
            
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name");
            ViewBag.TaskId = new SelectList(db.UserTasks.Where(d=> d.TaskId == id), "TaskId", "Message");
            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(AuthUserId)), "Id", "FullName");

            return View();
        }

        // POST: UserComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "Id,UserComments,TaskId,UserId,StatusId,file")] UserComment userComment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                userComment.file = UploadFile(file);
                db.UserComments.Add(userComment);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userComment.StatusId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userComment.TaskId);
            return View(userComment);
        }
        // GET: UserComments/Create
        public ActionResult Create(int? id)
        {

            var AuthUserId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();

            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name");
            ViewBag.TaskId = new SelectList(db.UserTasks.Where(d => d.TaskId == id), "TaskId", "Message");
            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(AuthUserId)), "Id", "FullName");

           
            ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == id).Include(d => d.AspNetUser).ToList();
            ViewBag.comments = db.UserComments.Where(d => d.TaskId == id).Include(d => d.AspNetUser).Include(d => d.Status).ToList();
            ViewBag.feedbacks = db.Feedbacks.Where(d => d.UserTaskId == id).ToList();


            return View();
        }

        // POST: UserComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,UserComments,TaskId,UserId,StatusId,file")] UserComment userComment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                String toId = db.UserTasks.Where(d => d.TaskId == userComment.TaskId).Select(d => d.CreatorId).FirstOrDefault();
                string userEmail = db.AspNetUsers.Where(x => x.Id == toId).Select(d => d.Email).FirstOrDefault();
                string userFullName = db.AspNetUsers.Where(x => x.Id == toId).Select(d => d.FullName).FirstOrDefault();
                string userTask = db.UserTasks.Where(x => x.TaskId == userComment.TaskId).Select(d => d.Message).FirstOrDefault();
                string senderId = User.Identity.GetUserId();
                string AssigerName = db.AspNetUsers.Where(x => x.Id == senderId).Select(n => n.FullName).FirstOrDefault();
                String status = db.Status.Where(d => d.StatusId == userComment.StatusId).Select(d => d.Name).FirstOrDefault();

                userComment.file = UploadFile(file);
                db.UserComments.Add(userComment);
                db.SaveChanges();
                //String result = sendEmail(userEmail, "Task Status Updated", "Dear " + userFullName + "," + AssigerName + " Updated Task : " + userTask + " Status to " + status);

                String result = sendEmail(userEmail, " New Comment Added on Task you assigned ", " Dear " + userFullName + ",<br/><p>" + AssigerName + " Added new Comment on Task : " + userTask + "</p>. <br/>Comment is : " + userComment.UserComments + "<p>  And Updated Status to : <b style='color: red'>  " + status + "</b> </p> <br/> <hr/>Check at: https://office.nsc.gov.af" + " | " + "<br/>https://office.nsc.gov.af:8080");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userComment.StatusId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userComment.TaskId);
            return View(userComment);
        }
        public String sendEmail(String to, String sub, String body)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(to);
            mm.Subject = sub;
            mm.Body = body;
            mm.From = new MailAddress("office@nsc.gov.af");

            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("mail.nsc.gov.af");
            smtp.Port = 25;

            //smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = false;
            smtp.Credentials = new System.Net.NetworkCredential("office@nsc.gov.af", "JSZD8bQ2FZoLvUijjQpa");



            smtp.Send(mm);
            return "success";
        }
     
        //Upload Image
        public string UploadFile(HttpPostedFileBase file)
        {
            string path = "-1";

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string filename = Path.GetFileName(file.FileName);
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

        // GET: UserComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserComment userComment = db.UserComments.Find(id);
            if (userComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userComment.StatusId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userComment.TaskId);
            return View(userComment);
        }

        // POST: UserComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserComments,TaskId,UserId,StatusId")] UserComment userComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userComment.StatusId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userComment.TaskId);
            return View(userComment);
        }

        // GET: UserComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserComment userComment = db.UserComments.Find(id);
            if (userComment == null)
            {
                return HttpNotFound();
            }
            return View(userComment);
        }

        // POST: UserComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserComment userComment = db.UserComments.Find(id);
            db.UserComments.Remove(userComment);
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
