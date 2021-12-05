using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OfficeFinal.Models;

namespace OfficeFinal.Controllers
{
    [Authorize]
    public class UserTaskRelationsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: UserTaskRelations
        public ActionResult Index()
        {
            var userTaskRelations = db.UserTaskRelations.Include(u => u.AspNetUser).Include(u => u.UserTask);
            return View(userTaskRelations.ToList());
        }

        // GET: UserTaskRelations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTaskRelation userTaskRelation = db.UserTaskRelations.Find(id);
            if (userTaskRelation == null)
            {
                return HttpNotFound();
            }
            return View(userTaskRelation);
        }


           [HttpPost]
        public JsonResult addPeopleJson(String userID, int taskID)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserTaskRelation taskRelation = new UserTaskRelation();
                taskRelation.UserId = userID;
                taskRelation.TaskId = taskID;
                string userEmail = db.AspNetUsers.Where(x => x.Id == userID).Select(d => d.Email).FirstOrDefault();
                string userFullName = db.AspNetUsers.Where(x => x.Id == userID).Select(d => d.FullName).FirstOrDefault();
                string userTask = db.UserTasks.Where(x => x.TaskId == taskID).Select(d => d.Message).FirstOrDefault();
                string usert = StripTagsRegex(userTask);
                string AssignerID = User.Identity.GetUserId();
                string AssigerName = db.AspNetUsers.Where(x => x.Id == AssignerID).Select(n => n.FullName).FirstOrDefault();
                db.UserTaskRelations.Add(taskRelation);
                db.SaveChanges();

                //ViewData["assigned"] = from u in db.AspNetUsers
                //                       join ut in db.UsersWithTasks on u.Id equals ut.TaskCreatorId
                //                       join t in db.OTasks on ut.TaskId equals t.Id
                //                       select new
                //                       {
                //                           u.Id,
                //                           u.UserName,
                //                           u.Email
                //                       };

                ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == taskID).Include(d => d.AspNetUser).ToList();
                String result = sendEmail(userEmail, "New Task Added to You", "<p>Dear " + userFullName + ",</p><br/><b style='color: green'> " + AssigerName + "</b>  Assigned you a New task : " + usert + "<hr/> Check at : https://office.nsc.gov.af" + " | " + "<br/>https://office.nsc.gov.af:8080");

                return Json(userID, JsonRequestBehavior.AllowGet);
            }

            return Json("error", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(int? id)
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.TaskId = new SelectList(db.UserTasks.Where(t => t.TaskId == id), "TaskId", "Message");

            var selectedTask = db.UserTasks.Where(d => d.TaskId == id).Include(s => s.Status).FirstOrDefault();
            return View();
        }

        // POST: UserTaskRelations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,TaskId")] UserTaskRelation userTaskRelation)
        {
            if (ModelState.IsValid)
            {
                db.UserTaskRelations.Add(userTaskRelation);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userTaskRelation.UserId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userTaskRelation.TaskId);
            return View(userTaskRelation);
        }

        public ActionResult AddPeople(int? id)
        {
            //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName");

         
            ViewBag.TaskId = new SelectList(db.UserTasks.Where(t => t.TaskId == id), "TaskId", "Message");

            var selectedTask = db.UserTasks.Where(d => d.TaskId == id).Include(s => s.Status).FirstOrDefault();
            ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == id).Include(d => d.AspNetUser).ToList();

            return View();
        }

        // POST: UserTaskRelations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPeople(String userID, int taskID)
        {
            if (User.Identity.IsAuthenticated)
            {

                UserTaskRelation taskRelation = new UserTaskRelation();
                taskRelation.UserId = userID;
                taskRelation.TaskId = taskID;
                string userEmail = db.AspNetUsers.Where(x => x.Id == userID).Select(d => d.Email).FirstOrDefault();
                string userFullName = db.AspNetUsers.Where(x => x.Id == userID).Select(d => d.FullName).FirstOrDefault();
                string userTask = db.UserTasks.Where(x => x.TaskId == taskID).Select(d => d.Message).FirstOrDefault();
                string usert = StripTagsRegex(userTask);
                string AssignerID = User.Identity.GetUserId();
                string AssigerName = db.AspNetUsers.Where(x => x.Id == AssignerID).Select(n => n.FullName).FirstOrDefault();
                db.UserTaskRelations.Add(taskRelation);
                db.SaveChanges();

                //String result = sendEmail(userEmail, "Your New Task", "Dear " + userFullName + "," + AssigerName + "  Assign you a New task: " + userTask +"Use this link: http://office.nsc.gov.af");

                String result = sendEmail(userEmail, "New Task Added to You", "<p>Dear " + userFullName + ",</p><br/><b style='color: green'> " + AssigerName + "</b>  Assigned you a New task : " + usert + "<hr/> Check at : https://office.nsc.gov.af" + " | " + "<br/>https://office.nsc.gov.af:8080");

                //ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == taskID).Include(d => d.AspNetUser).ToList();
                return RedirectToAction("AddPeople");
            }

            return View();
        }
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
        //Email
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
        // GET: UserTaskRelations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTaskRelation userTaskRelation = db.UserTaskRelations.Find(id);
            if (userTaskRelation == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userTaskRelation.UserId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userTaskRelation.TaskId);
            return View(userTaskRelation);
        }

        // POST: UserTaskRelations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,TaskId")] UserTaskRelation userTaskRelation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userTaskRelation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", userTaskRelation.UserId);
            ViewBag.TaskId = new SelectList(db.UserTasks, "TaskId", "CreatorId", userTaskRelation.TaskId);
            return View(userTaskRelation);
        }

        // GET: UserTaskRelations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTaskRelation userTaskRelation = db.UserTaskRelations.Find(id);
            if (userTaskRelation == null)
            {
                return HttpNotFound();
            }
            return View(userTaskRelation);
        }

        // POST: UserTaskRelations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTaskRelation userTaskRelation = db.UserTaskRelations.Find(id);
            db.UserTaskRelations.Remove(userTaskRelation);
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
