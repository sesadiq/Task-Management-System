using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OfficeFinal.Models;
using OfficeFinal.ViewModels;

namespace OfficeFinal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();
        public ActionResult Index()
        {
            
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();

            var myIdsOnTaks = db.UserTaskRelations.Where(d => d.UserId.Equals(myId)).Select(d => d.TaskId).ToList();
            //ViewBag.files = db.UserTasks.Where(d => d.AspNetUser.Id.Equals(myId)).Include(d => d.FileDetails).ToList();
           List<UserTask> userTasks= null;

            //for (int i = 0;  i< myIdsOnTaks.Count; i++)
            //{
            //   userTasks = db.UserTasks.Where(d => d.TaskId == myIdsOnTaks[i]).Include(u => u.AspNetUser).Include(u => u.Status).ToList();

            //}


            var Tasks = from t in db.UserTasks
                        from ut in db.UserTaskRelations
                        where ut.UserId == myId & ut.TaskId == t.TaskId
                        orderby t.TaskId descending
                        select t;

            ViewBag.yourAssignedTask = db.UserTasks.Where(d => d.AspNetUser.Id == myId).OrderByDescending(x=>x.TaskId).Include(u => u.AspNetUser).Include(u => u.Status).Include(u=>u.TaskType).ToList();

            return View(Tasks.ToList());
        }
        public FileResult Download(string d)
        {
            return File(Path.Combine(Server.MapPath("~/Content/UploadedFiles"),d), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        public ActionResult About()
        {
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();

            var userTasks = db.UserTasks.Where(d => d.AspNetUser.Id == myId).Include(u => u.AspNetUser).Include(u => u.Status);
            ViewBag.Message = "Your application description page.";

            return View(userTasks.ToList());
        }
        public ActionResult Details(int id)
        {
            ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == id).Include(d => d.AspNetUser).ToList();
            var taskDetail = db.UserTasks.Where(d => d.TaskId == id).Include(d => d.Status).Include(d => d.TaskType).FirstOrDefault();
            ViewBag.comments = db.UserComments.Where(d => d.TaskId == id).Include(d => d.AspNetUser).Include(d => d.Status).ToList();
            ViewBag.feedbacks = db.Feedbacks.Where(d => d.UserTaskId == id).ToList();

            return View(taskDetail);
        }

        public ActionResult FullTaskEdit(int id)
        {
            ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == id).Include(d => d.AspNetUser).ToList();
            var taskDetail = db.UserTasks.Where(d => d.TaskId == id).Include(d => d.Status).FirstOrDefault();
            ViewBag.comments = db.UserComments.Where(d => d.TaskId == id).Include(d => d.AspNetUser).Include(d => d.Status).ToList();
            ViewBag.feedbacks = db.Feedbacks.Where(d => d.UserTaskId == id).ToList();


            return View(taskDetail);
        }
        public ActionResult EditTask(int? id, string cId)
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
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();
            ViewBag.CreatorId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(cId)), "Id", "Email");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userTask.StatusId);
            ViewBag.TaskTypeId = new SelectList(db.TaskTypes, "Id", "Type");
            return View(userTask);
        }

        // POST: UserTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditTask([Bind(Include = "TaskId,CreatorId,Message,CreateDate,DeadLine,StatusId,files")] UserTask userTask, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (userTask.files == null)
                {
                    var filename = db.UserTasks.Where(x => x.TaskId == userTask.TaskId).Select(x => x.files).FirstOrDefault();
                    userTask.files = filename;
                    db.Entry(userTask).State = EntityState.Modified;
                    db.SaveChanges();
                    //int createdTaskId = db.UserTasks.Where(d => d.Message.Equals(userTask.Message) && d.DeadLine == userTask.DeadLine).Select(d => d.TaskId).FirstOrDefault();

                    return RedirectToAction("FullTaskEdit", "Home", new { id = userTask.TaskId });
                }
                else
                {
                    userTask.files = UploadFile(files);
                    db.Entry(userTask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("FullTaskEdit", "Home", new { id = userTask.TaskId });
                }
                
            }
            ViewBag.CreatorId = new SelectList(db.AspNetUsers, "Id", "Email", userTask.CreatorId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Name", userTask.StatusId);
            return View(userTask);
        }
        public ActionResult AddPeople(int? id)
        {
            //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName");
            ViewBag.TaskId = new SelectList(db.UserTasks.Where(t => t.TaskId == id), "TaskId", "Message");
            ViewData["id"] = id;
            var selectedTask = db.UserTasks.Where(d => d.TaskId == id).Include(s => s.Status).FirstOrDefault();
            ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == id).Include(d => d.AspNetUser).ToList();
            

            return View();
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

                //String result = sendEmail(userEmail, "Your New Task", "Dear " + userFullName + "," + AssigerName + "  Assign you a New task: " + usert);
                String result = sendEmail(userEmail, "New Task Added to You", "<p>Dear " + userFullName + ",</p><br/><b style='color: green'> " + AssigerName + "</b>  Assigned you a New task : " + usert + "<hr/> Check at : https://office.nsc.gov.af" + " | " + "<br/>https://office.nsc.gov.af:8080");

                //ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == taskID).Include(d => d.AspNetUser).ToList();
                return RedirectToAction("FullTaskEdit", "Home", new { id = taskID });
            }

            return View();
        }
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
        public ActionResult DeleteTask(int? id)
        {
            UserTask userTask = db.UserTasks.Find(id);
            List<UserTaskRelation> userTaskRelation = db.UserTaskRelations.Where(d => d.TaskId == id).ToList();
            List<UserComment> userComments = db.UserComments.Where(d => d.TaskId == id).ToList();
            List<Feedback> feedbacks = db.Feedbacks.Where(d => d.UserTaskId == id).ToList();

            db.UserComments.RemoveRange(userComments);

            db.UserTaskRelations.RemoveRange(userTaskRelation);

            db.UserTasks.Remove(userTask);

            db.Feedbacks.RemoveRange(feedbacks);

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete2(String id, int? tId)
        {

            UserTaskRelation userTaskRelation = db.UserTaskRelations.Where(d => d.TaskId == tId && d.UserId.Equals(id)).FirstOrDefault();
            db.UserTaskRelations.Remove(userTaskRelation);
            db.SaveChanges();

            //ViewBag.usersList = db.UserTaskRelations.Where(d => d.TaskId == tId).Include(d => d.AspNetUser).ToList();
            //var taskDetail = db.UserTasks.Where(d => d.TaskId == id).Include(d => d.Status).FirstOrDefault();
            //ViewBag.comments = db.UserComments.Where(d => d.TaskId == id).Include(d => d.AspNetUser).Include(d => d.Status).ToList();

            return RedirectToAction("FullTaskEdit", "Home", new { id = tId });
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

        //private string sendEmail(string userEmail, string v1, string v2)
        //{
        //    throw new NotImplementedException();
        //}

        public ActionResult Edit(int? id, string cId)
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
            String myId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();
            ViewBag.CreatorId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(cId)), "Id", "Email");
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
    }
}