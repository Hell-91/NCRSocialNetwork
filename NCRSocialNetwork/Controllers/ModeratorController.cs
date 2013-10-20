using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCRSocialNetwork.Models;

namespace NCRSocialNetwork.Controllers
{
    public class ModeratorController : Controller
    {
        private NCRSNEntity db = new NCRSNEntity();

        //
        // GET: /Moderator/

        public ActionResult Index()
        {
            var clubmoderators = db.ClubModerators.Include(c => c.Club).Include(c => c.User);
            return View(clubmoderators.ToList());
        }

        //
        // GET: /Moderator/Details/5

        public ActionResult Details(int id = 0)
        {
            ClubModerator clubmoderator = db.ClubModerators.Find(id);
            if (clubmoderator == null)
            {
                return HttpNotFound();
            }
            return View(clubmoderator);
        }

        //
        // GET: /Moderator/Create

        public ActionResult Create()
        {
            ViewBag.ClubModeratorClubID = new SelectList(db.Clubs, "ClubId", "ClubName");
            ViewBag.ClubModeratorUserId = new SelectList(db.Users, "UserId", "UserFirstName");
            return View();
        }

        //
        // POST: /Moderator/Create

        [HttpPost]
        public ActionResult Create(ClubModerator clubmoderator, string ModeratorUserId)
        {
            string qId = ModeratorUserId;
            clubmoderator.ClubModeratorUserId = db.Users.Where(u => u.UserQuicklookId == qId).First().UserId;
            if (db.ClubModerators.Where(m => m.ClubModeratorClubID == clubmoderator.ClubModeratorClubID && m.ClubModeratorUserId == clubmoderator.ClubModeratorUserId).Count() > 0)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (ModelState.IsValid)
            {
                db.ClubModerators.Add(clubmoderator);
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.ClubModeratorClubID = new SelectList(db.Clubs, "ClubId", "ClubName", clubmoderator.ClubModeratorClubID);
            ViewBag.ClubModeratorUserId = new SelectList(db.Users, "UserId", "UserFirstName", clubmoderator.ClubModeratorUserId);
            return View(clubmoderator);
        }

        //
        // GET: /Moderator/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ClubModerator clubmoderator = db.ClubModerators.Find(id);
            if (clubmoderator == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubModeratorClubID = new SelectList(db.Clubs, "ClubId", "ClubName", clubmoderator.ClubModeratorClubID);
            ViewBag.ClubModeratorUserId = new SelectList(db.Users, "UserId", "UserFirstName", clubmoderator.ClubModeratorUserId);
            return View(clubmoderator);
        }

        //
        // POST: /Moderator/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubModerator clubmoderator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clubmoderator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubModeratorClubID = new SelectList(db.Clubs, "ClubId", "ClubName", clubmoderator.ClubModeratorClubID);
            ViewBag.ClubModeratorUserId = new SelectList(db.Users, "UserId", "UserFirstName", clubmoderator.ClubModeratorUserId);
            return View(clubmoderator);
        }

        //
        // GET: /Moderator/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ClubModerator clubmoderator = db.ClubModerators.Find(id);
            if (clubmoderator == null)
            {
                return HttpNotFound();
            }
            return View(clubmoderator);
        }

        //
        // POST: /Moderator/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClubModerator clubmoderator = db.ClubModerators.Find(id);
            db.ClubModerators.Remove(clubmoderator);
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