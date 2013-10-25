using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCRSocialNetwork.Models;
using NCRSocialNetwork.ViewModels;

namespace NCRSocialNetwork.Controllers
{
    public class FeedbackController : Controller
    {
        private NCRSNEntity db = new NCRSNEntity();

        //
        // GET: /Feedback/

        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks.Include(f => f.User);
            return View(feedbacks.ToList());
        }

        //
        // GET: /Feedback/Details/5

        public ActionResult Details(int id = 0)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        //
        // GET: /Feedback/Create

        public ActionResult PostFeedback()
        {
            FeedbackViewModel viewModel = new FeedbackViewModel();
            viewModel.BaseUser = new User();
            viewModel.BaseClub = new List<Club>();
            viewModel.Feedback = new List<Feedback>();
            viewModel.User = new List<User>();

            var name = User.Identity.Name.Split('\\')[1];
            
            if (db.Clubs.ToList().Count() > 0)
            {
                viewModel.BaseClub = db.Clubs.ToList();
            }
            if (db.Users.ToList().Count() > 0)
            {
                viewModel.BaseUser = db.Users.Where(u => u.UserQuicklookId == name).First();
            }
            if (db.Feedbacks.Count() > 0)
            {
                viewModel.Feedback = db.Feedbacks.ToList();
                foreach (var feedback in viewModel.Feedback)
                {
                    viewModel.User.Add(feedback.User);                
                }
            }

            return View(viewModel);
        }

        //
        // POST: /Feedback/Create

        [HttpPost]
        public ActionResult PostFeedback(Feedback feedback)
        {
            feedback.FeedbackCreatedDate = DateTime.Now.ToString();
            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.FeedbackUserId = new SelectList(db.Users, "UserId", "UserFirstName", feedback.FeedbackUserId);
            return View(feedback);
        }

        //
        // GET: /Feedback/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.FeedbackUserId = new SelectList(db.Users, "UserId", "UserFirstName", feedback.FeedbackUserId);
            return View(feedback);
        }

        //
        // POST: /Feedback/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FeedbackUserId = new SelectList(db.Users, "UserId", "UserFirstName", feedback.FeedbackUserId);
            return View(feedback);
        }

        //
        // GET: /Feedback/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        //
        // POST: /Feedback/Delete/5

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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}