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
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private NCRSNEntity dbConn = new NCRSNEntity();

        public ActionResult Index()
        {
           
            var viewModel = new AdminViewModel();

            var name = User.Identity.Name.Split('\\')[1];
            viewModel.Clubs = new List<Club>();
            viewModel.Events = new List<Event>();
            viewModel.EventLikeDislikes = new List<EventLikeDislike>();
            viewModel.BaseClub = new List<Club>();
            viewModel.BaseUser = new User();
            viewModel.EventRequests = new List<EventRequest>();
            viewModel.Users = new List<User>();
            viewModel.ClubModerators = new List<ClubModerator>();

            if (dbConn.Clubs.ToList().Count() > 0)
            {
                viewModel.Clubs = dbConn.Clubs.ToList();
            }
            if (dbConn.Events.ToList().Count() > 0)
            {
                viewModel.Events = dbConn.Events.ToList();
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
            if (dbConn.EventRequests.ToList().Count() > 0) 
            {
                viewModel.EventRequests = dbConn.EventRequests.ToList();
            }
            if (dbConn.Users.ToList().Count() > 0)
            {
                viewModel.Users = dbConn.Users.ToList();
            }
            if (dbConn.Users.ToList().Count() > 0)
            {
                viewModel.ClubModerators = dbConn.ClubModerators.ToList();
                var temp = viewModel.ClubModerators;
                viewModel.ClubModerators.Clear();
                foreach (var v in temp)
                {
                    if (v.User.UserRole != "Admin")
                    {
                        viewModel.ClubModerators.Add(v);
                    }
                }
            }

            return View(viewModel);
        }

        //
        // GET: /Admin/Details/5

        public ActionResult Details(int id = 0)
        {
            Event Event = dbConn.Events.Find(id);
            if (Event == null)
            {
                return HttpNotFound();
            }
            return View(Event);
        }


        protected override void Dispose(bool disposing)
        {
            dbConn.Dispose();
            base.Dispose(disposing);
        }
    }
}