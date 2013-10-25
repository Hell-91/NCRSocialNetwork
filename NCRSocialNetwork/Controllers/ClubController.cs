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
    public class ClubController : Controller
    {
        private NCRSNEntity db = new NCRSNEntity();

        //
        // GET: /Club/

        public ActionResult Index()
        {
            var clubs = db.Clubs.Include(c => c.Key);
            return View(clubs.ToList());
        }

        //
        // GET: /Club/Details/5

        public ActionResult Details(int id = 0)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        //
        // GET: /Club/5

        public ActionResult ClubPage(string clubName)
        {
            var clubId = db.Clubs.Where(c => c.ClubName == clubName).First().ClubId;
            var name = User.Identity.Name.Split('\\')[1];
            bool clubSubscribedFlag = false;
            if (db.ClubSubscribes.Where(c => c.ClubSubscribeClubId == clubId && c.User.UserQuicklookId == name).Count() > 0) {
                clubSubscribedFlag = true;
            }

            var events = new List<Event>();
            if (db.Events.Where(e => e.EventClubId == clubId).ToList().Count() > 0)
            {
                events = db.Events.Where(e => e.EventClubId == clubId).ToList();
            }
            
            var clubComments = new List<Comment>();
            foreach (var comment in db.Comments.ToList())
            {
                foreach (var e in events) {
                    if (comment.CommentKeyId == e.EventId) {
                        clubComments.Add(comment);
                        break;
                    }
                }
            }

            ClubViewModel viewModel = new ClubViewModel();
            
            viewModel.Club = db.Clubs.Find(clubId);
            viewModel.Events = new List<Event>();
            viewModel.Comments = new List<Comment>();
            viewModel.EventLikeDislikes = new List<EventLikeDislike>();
            viewModel.BaseClub = new List<Club>();
            viewModel.BaseUser = new User();
            viewModel.UsersSubscribed = new List<ClubSubscribe>();
            viewModel.ClubModerators = new List<ClubModerator>();

            viewModel.ClubSubscribed = clubSubscribedFlag;
            viewModel.Events = events;
            viewModel.Comments = clubComments;
            viewModel.isModerator = false;

            if (db.EventLikeDislikes.Where(e => e.Event.EventClubId == clubId).ToList().Count() > 0)
            {
                viewModel.EventLikeDislikes = db.EventLikeDislikes.Where(e => e.Event.EventClubId == clubId).ToList();
            }
            if (db.Clubs.ToList().Count() > 0)
            {
                viewModel.BaseClub = db.Clubs.ToList();
            }
            if (db.Users.ToList().Count() > 0)
            {
                viewModel.BaseUser = db.Users.Where(u => u.UserQuicklookId == name).First();
            }
            if (db.ClubSubscribes.ToList().Count() > 0)
            {
                viewModel.UsersSubscribed = db.ClubSubscribes.Where(c => c.ClubSubscribeClubId == clubId).ToList();
            }
            if (db.ClubModerators.ToList().Count() > 0)
            {
                viewModel.ClubModerators = db.ClubModerators.Where(c => c.ClubModeratorClubID == clubId).ToList();
                viewModel.isModerator = viewModel.ClubModerators.Where(c => c.User.UserQuicklookId == name).Count() > 0 ? true : false;
            }
            
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Clubs/Join

        [HttpPost]
        public ActionResult SubscribeClub(int clubId, int userId)
        {
            ClubSubscribe clubsubscribe = new ClubSubscribe()
            {
                ClubSubscribeClubId = clubId,
                ClubSubscribeUserId = userId
            };

            if (ModelState.IsValid)
            {
                db.ClubSubscribes.Add(clubsubscribe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubSubscribeUserId = new SelectList(db.Users, "UserId", "UserFirstName", clubsubscribe.ClubSubscribeUserId);
            ViewBag.ClubSubscribeClubId = new SelectList(db.Clubs, "ClubId", "ClubName", clubsubscribe.ClubSubscribeClubId);
            return View(clubsubscribe);
        }

        //
        // POST: /TestJoin/Delete/5

        [HttpPost]
        public ActionResult UnsubscribeClub(int clubId, int userId)
        {
            int id = db.ClubSubscribes.Where(e => e.ClubSubscribeClubId == clubId && e.ClubSubscribeUserId == userId).First().ClubSubscribeId;
            ClubSubscribe clubsubscribe = db.ClubSubscribes.Find(id);
            db.ClubSubscribes.Remove(clubsubscribe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Club/Create

        public ActionResult Create()
        {
            ViewBag.ClubModerator = new SelectList(db.Users, "UserId", "UserFirstName");
            ViewBag.ClubId = new SelectList(db.Keys, "KeyId", "KeyType");
            return View();
        }

        [HttpPost]
        public PartialViewResult SubmitComment(int UserId, string UserName, string UserComment, int EventId)
        {
            var eventcomment = new Comment()
            {
                CommentUserId = UserId,
                CommentDescription = UserComment,
                CommentKeyId = EventId,
                CommentCreatedDate = DateTime.Now
            };
            if (ModelState.IsValid)
            {
                db.Comments.Add(eventcomment);
                db.SaveChanges();
            }
            ViewBag.UserId = UserId;
            ViewBag.Comment = UserName + ": " + UserComment;
            ViewBag.Imagelink = db.Users.Where(u => u.UserId == UserId).First().UserDisplayPicture;
            return PartialView("_EventCommentView");
        }


        //
        // POST: /Club/Create

        [HttpPost]
        public ActionResult Create(Club club)
        {
            Key key = new Key()
            {
                KeyType = "C",
                KeyCreatedDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                db.Keys.Add(key);
                db.SaveChanges();
            }

            var qId = User.Identity.Name.Split('\\')[1];
            club.ClubId = key.KeyId;
            club.Key = key;
              
            ClubModerator clubModerator = new ClubModerator();
            clubModerator.ClubModeratorClubID = key.KeyId;
            clubModerator.ClubModeratorUserId = db.Users.Where(u => u.UserQuicklookId == qId).First().UserId;

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];
                //Save file here
                if (hpf.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(hpf.FileName);
                    var path = Path.Combine(Server.MapPath("/NCRSocialNetwork/Uploads"), fileName);
                    hpf.SaveAs(path);
                    if (file == "ClubThumbnailPath")
                    {
                        club.ClubThumbnailPath = "/NCRSocialNetwork/Uploads/" + fileName;
                    }
                    else
                    {
                        club.ClubImagePath = "/NCRSocialNetwork/Uploads/" + fileName;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                db.Clubs.Add(club);
                db.ClubModerators.Add(clubModerator);
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }

           // ViewBag.ClubModerator = new SelectList(db.Users, "UserId", "UserFirstName", club.ClubModerator);
            ViewBag.ClubId = new SelectList(db.Keys, "KeyId", "KeyType", club.ClubId);
            return View(club);
        }

        //
        // GET: /Club/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
           // ViewBag.ClubModerator = new SelectList(db.Users, "UserId", "UserFirstName", club.ClubModerator);
            ViewBag.ClubId = new SelectList(db.Keys, "KeyId", "KeyType", club.ClubId);
            return View(club);
        }

        //
        // POST: /Club/Edit/5

        [HttpPost]
        public ActionResult Edit(Club club)
        {
            string page = "Admin";

            string requestedPage = HttpContext.Request.UrlReferrer.ToString();
            if (requestedPage.Contains("Clubs"))
            {
                page = "Clubs/" + club.ClubName;
            }

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];
                //Save file here
                if (hpf.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(hpf.FileName);
                    var path = Path.Combine(Server.MapPath("/NCRSocialNetwork/Uploads"), fileName);
                    hpf.SaveAs(path);
                    if (file == "ClubThumbnailPath")
                    {
                        club.ClubThumbnailPath = "/NCRSocialNetwork/Uploads/" + fileName;
                    }
                    else 
                    {
                        club.ClubImagePath = "/NCRSocialNetwork/Uploads/" + fileName;
                    }
                }
            }

            NCRSNEntity tempDb = new NCRSNEntity();
            if (club.ClubImagePath == null) {
                club.ClubImagePath = tempDb.Clubs.Where(c => c.ClubId == club.ClubId).First().ClubImagePath;
            }

            if (club.ClubThumbnailPath == null)
            {
                club.ClubThumbnailPath = tempDb.Clubs.Where(c => c.ClubId == club.ClubId).First().ClubThumbnailPath;
            }

            tempDb.Dispose();

            if (ModelState.IsValid)
            {
                db.Entry(club).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(page, "/");
            }
           // ViewBag.ClubModerator = new SelectList(db.Users, "UserId", "UserFirstName", club.ClubModerator);
            ViewBag.ClubId = new SelectList(db.Keys, "KeyId", "KeyType", club.ClubId);
            return View(club);
        }

        //
        // GET: /Club/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        //
        // POST: /Club/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Club club = db.Clubs.Find(id);
            db.Clubs.Remove(club);
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