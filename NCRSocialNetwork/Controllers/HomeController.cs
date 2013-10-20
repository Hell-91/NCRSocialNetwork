using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCRSocialNetwork.ViewModels;
using NCRSocialNetwork.Models;
using System.Data;
using System.Data.Entity;

namespace NCRSocialNetwork.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private NCRSNEntity dbConn = new NCRSNEntity();

        public int Authorize(string username)
        {
            int returnValue = 0;
            if ((returnValue = dbConn.Users.Where(u => u.UserQuicklookId == username).Count()) > 0)
            {
                return returnValue;
            }
            else
            {
                return returnValue;
            }
        }

        public ActionResult Index()
        {
            var name = User.Identity.Name.Split('\\')[1];
            var id = Authorize(name);
            if (id == 0)
            {
                return RedirectToAction("Create", "Profile");                
            }
            var viewModel = new HomeViewModel();
            viewModel.Clubs = new List<Club>();
            viewModel.Events = new List<Event>();
            viewModel.Comments = new List<Comment>();
            viewModel.EventLikeDislikes = new List<EventLikeDislike>();
            viewModel.BaseClub = new List<Club>();
            viewModel.BaseUser = new User();

            if (dbConn.Clubs.ToList().Count() > 0)
            {
                viewModel.Clubs = dbConn.Clubs.ToList();                
            }
            if (dbConn.Events.ToList().Count() > 0)
            {
                viewModel.Events = dbConn.Events.ToList();
            }
            if (dbConn.Comments.ToList().Count() > 0)
            {
                viewModel.Comments = dbConn.Comments.ToList();
            }
            if (dbConn.EventLikeDislikes.ToList().Count() > 0)
            {
                viewModel.EventLikeDislikes = dbConn.EventLikeDislikes.ToList();
            }
            if (dbConn.Clubs.ToList().Count() > 0)
            {
                viewModel.BaseClub = dbConn.Clubs.ToList();
            }
            if (dbConn.Users.ToList().Count() > 0)
            {
                viewModel.BaseUser = dbConn.Users.Where(u => u.UserQuicklookId == name).First();
            } 

            return View(viewModel);
        }

        [HttpPost]
        public PartialViewResult SubmitComment(int UserId, string UserName, string UserComment, int EventId)
        {
            var eventcomment = new Comment() { 
                CommentUserId = UserId,
                CommentDescription = UserComment,
                CommentKeyId = EventId,
                CommentCreatedDate = DateTime.Now
            };            
            if (ModelState.IsValid)
            {
                dbConn.Comments.Add(eventcomment);
                dbConn.SaveChanges();
            }
            ViewBag.Comment = UserName + ": " + UserComment;
            ViewBag.Imagelink = dbConn.Users.Where(u => u.UserId == UserId).First().UserDisplayPicture;
            return PartialView("_EventCommentView");
        }

        [HttpPost]
        public PartialViewResult LikeEvent(int UserId, int EventId)
        {
            var eventlike = new EventLikeDislike(){
                EventLikeDislikeUserId = UserId,
                EventLikeDislikeEventId = EventId,
                EventLikeDislikeValue = 1
            };
            if (ModelState.IsValid)
            {
                dbConn.EventLikeDislikes.Add(eventlike);
                dbConn.SaveChanges();
            }
            ViewBag.Likes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 1);
            ViewBag.Dislikes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 0);
            return PartialView("_LikeDislikeView");
        }

        [HttpPost]
        public PartialViewResult DislikeEvent(int UserId, int EventId)
        {
            var eventlike = new EventLikeDislike()
            {
                EventLikeDislikeUserId = UserId,
                EventLikeDislikeEventId = EventId,
                EventLikeDislikeValue = 0
            };
            if (ModelState.IsValid)
            {
                dbConn.EventLikeDislikes.Add(eventlike);
                dbConn.SaveChanges();
            }
            ViewBag.Likes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 1);
            ViewBag.Dislikes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 0);
            return PartialView("_LikeDislikeView");
        }

        [HttpPost]
        public PartialViewResult UnlikeEvent(int UserId, int EventId)
        {
            var eventLikeDislike = dbConn.EventLikeDislikes.Where(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeUserId == UserId).FirstOrDefault();
            EventLikeDislike eventlikedislike = dbConn.EventLikeDislikes.Find(eventLikeDislike.EventLikeDislikeId);
            dbConn.EventLikeDislikes.Remove(eventlikedislike);
            dbConn.SaveChanges();

            ViewBag.Likes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 1);
            ViewBag.Dislikes = dbConn.EventLikeDislikes.Count(e => e.EventLikeDislikeEventId == EventId && e.EventLikeDislikeValue == 0);
            return PartialView("_LikeDislikeView");
        }
    }
}
