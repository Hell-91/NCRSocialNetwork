using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCRSocialNetwork.Models;
using NCRSocialNetwork.ViewModels;
using System.IO;

namespace NCRSocialNetwork.Controllers
{
    public class ProfileController : Controller
    {
        private NCRSNEntity db = new NCRSNEntity();

        //
        // GET: /Profile/

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /Profile/Details/5

        public ActionResult Details(int userId = 0)
        {
            User user = db.Users.Find(userId);
            string UserQuicklookId = User.Identity.Name.Split('\\')[1];

            ProfileViewModel profile = new ProfileViewModel() { 
                Profile = user,
                Comments = db.Comments.Where(c => c.CommentUserId == userId).ToList(),
                EventLikeDislikes = db.EventLikeDislikes.Where(e => e.EventLikeDislikeUserId == userId).ToList(),
                BaseClub = db.Clubs.ToList(),
                BaseUser = db.Users.Where(u => u.UserQuicklookId == UserQuicklookId).First()
            };

            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        //
        // GET: /Profile/Create

        public ActionResult Create()
        {
            var viewModel = new ProfileViewModel();
            viewModel.Comments = new List<Comment>();
            viewModel.EventLikeDislikes = new List<EventLikeDislike>();
            viewModel.BaseClub = new List<Club>();
            viewModel.BaseUser = new User();

            if (db.Comments.ToList().Count() > 0)
            {
                viewModel.Comments = db.Comments.ToList();
            }
            if (db.EventLikeDislikes.ToList().Count() > 0)
            {
                viewModel.EventLikeDislikes = db.EventLikeDislikes.ToList();
            }
            if (db.Clubs.ToList().Count() > 0)
            {
                viewModel.BaseClub = db.Clubs.ToList();
            }

            return View(viewModel);
        }

        //
        // POST: /Profile/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            user.UserQuicklookId = User.Identity.Name.Split('\\')[1];
            user.UserRole = "User";

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];
                //Save file here
                if (hpf.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(hpf.FileName);
                    var path = Path.Combine(Server.MapPath("/NCRSocialNetwork/Uploads"), fileName);
                    hpf.SaveAs(path);
                    user.UserDisplayPicture = "/NCRSocialNetwork/Uploads/" + fileName;
                }
            }

            if (user.UserDisplayPicture == null)
            {
                user.UserDisplayPicture = "/NCRSocialNetwork/Content/Images/profile-picture_01.png";
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        //
        // GET: /Profile/Edit/5

        public ActionResult UpdateProfile(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Profile/Edit/5

        [HttpPost]
        public ActionResult UpdateProfile(User user)
        {
            user.UserQuicklookId = User.Identity.Name.Split('\\')[1];

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];
                //Save file here
                if (hpf.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(hpf.FileName);
                    var path = Path.Combine(Server.MapPath("/NCRSocialNetwork/Uploads"), fileName);
                    hpf.SaveAs(path);
                    user.UserDisplayPicture = "/NCRSocialNetwork/Uploads/" + fileName;
                }
            }

            NCRSNEntity tempDb = new NCRSNEntity();
            user.UserRole = tempDb.Users.Find(user.UserId).UserRole;
            if (user.UserDisplayPicture == null)
            {
                user.UserDisplayPicture = tempDb.Users.Find(user.UserId).UserDisplayPicture;
            }
            tempDb.Dispose();

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(user.UserId.ToString(), "UserProfile");
            }
            return View(user);
        }

        //
        // GET: /Profile/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Profile/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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