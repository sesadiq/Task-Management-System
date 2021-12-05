using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeFinal.Models;

namespace OfficeFinal.Controllers
{
    public class FeedbacksController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Feedbacks
        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks.Include(f => f.AspNetUser).Include(f => f.UserComment);
            return View(feedbacks.ToList());
        }

        // GET: Feedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        public ActionResult DeleteFeedback(int id, int taskId)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
            db.SaveChanges();
            return RedirectToAction("FullTaskEdit", "Home", new { id = taskId });
        }
        // GET: Feedbacks/Create
        public ActionResult Create(int id, int taskId)
        {
            var AuthUserId = db.AspNetUsers.Where(d => d.Email.Equals(User.Identity.Name)).Select(d => d.Id).FirstOrDefault();

            ViewBag.UserTaskId = new SelectList(db.UserTasks.Where(d => d.TaskId == taskId), "TaskId", "Message");
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers.Where(d => d.Id.Equals(AuthUserId)), "Id", "FullName");
            ViewBag.UserCommentId = new SelectList(db.UserComments.Where(d => d.Id == id), "Id", "UserComments");
            return View();
        }


        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserFeedback,UserCommentId,UserTaskId,AspNetUserId")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                return RedirectToAction("Details","Home", new { id = feedback.UserTaskId});
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.AspNetUserId);
            ViewBag.UserCommentId = new SelectList(db.UserComments, "Id", "UserComments", feedback.UserCommentId);
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.AspNetUserId);
            ViewBag.UserCommentId = new SelectList(db.UserComments, "Id", "UserComments", feedback.UserCommentId);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserFeedback,UserCommentId,UserTaskId,AspNetUserId")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.AspNetUserId);
            ViewBag.UserCommentId = new SelectList(db.UserComments, "Id", "UserComments", feedback.UserCommentId);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
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
