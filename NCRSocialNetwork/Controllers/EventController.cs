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
    public class EventController : Controller
    {
        private NCRSNEntity db = new NCRSNEntity();

        //
        // GET: /Event/

        public ActionResult Index()
        {
            var Events = db.Events.Include(e => e.User).Include(e => e.Club).Include(e => e.Key);
            return View(Events.ToList());
        }

        //
        // GET: /Event/Details/5

        public ActionResult Details(int id = 0)
        {
            var name = User.Identity.Name.Split('\\')[1];
            Event requestedEvent = db.Events.Find(id);

            var eventLikesDislikes = new List<EventLikeDislike>();
            if (db.EventLikeDislikes.Where(e => e.EventLikeDislikeEventId == id).Count() > 0) {
                eventLikesDislikes = db.EventLikeDislikes.Where(e => e.EventLikeDislikeEventId == id).ToList();
            }
            EventAttending uEA = new EventAttending();
            List<EventAttending> eA = db.EventAttendings.Where(e => e.EventAttendingEventId == id).ToList();
            if (eA.Where(e => e.User.UserQuicklookId == name).Count() > 0)
            {
                uEA = eA.Where(e => e.User.UserQuicklookId == name).First();
            }
            int uEAFlag = -2;
            if (uEA != null)
            {
                uEAFlag = uEA.EventAttendingValue; 
            }

            EventViewModel Event = new EventViewModel() { 
                Event = requestedEvent,
                BaseClub = db.Clubs.ToList(),
                EventLikeDislikes = eventLikesDislikes,
                BaseUser = db.Users.Where(u => u.UserQuicklookId == name).First(),
                Comments = db.Comments.Where(c => c.CommentKeyId == id).ToList(),
                EventAttending = eA,
                UserEventAttendingStatus = uEAFlag
            };
            if (Event == null)
            {
                return HttpNotFound();
            }
            return View(Event);
        }

        //
        // GET: /Event/Create

        public ActionResult Create()
        {
            ViewBag.EventCreatedBy = new SelectList(db.Users, "UserId", "UserFirstName");
            ViewBag.EventClubId = new SelectList(db.Clubs, "ClubId", "ClubName");
            ViewBag.EventId = new SelectList(db.Keys, "KeyId", "KeyType");
            return View();
        }

        //
        // POST: /Event/Create

        [HttpPost]
        public ActionResult Create(Event Event)
        {
            Event.EventCreatedTime = DateTime.Now;
            string page = "Admin";

            string requestedPage = HttpContext.Request.UrlReferrer.ToString();
            if (requestedPage.Contains("Club")) {
                page = "Clubs/" + Event.EventClubId.ToString() + "/";
            }

            Key key = new Key()
            {
                KeyType = "E",
                KeyCreatedDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                db.Keys.Add(key);
                db.SaveChanges();
            }

            Event.EventId = key.KeyId;

            if (ModelState.IsValid)
            {
                db.Events.Add(Event);
                db.SaveChanges();

                EventRequest eventrequest = db.EventRequests.Find(Event.EventEventRequestId);
                eventrequest.EventRequestFlag = "true";
                db.Entry(eventrequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", page);
            }

            ViewBag.EventCreatedBy = new SelectList(db.Users, "UserId", "UserFirstName", Event.EventCreatedBy);
            ViewBag.EventClubId = new SelectList(db.Clubs, "ClubId", "ClubName", Event.EventClubId);
            ViewBag.EventId = new SelectList(db.Keys, "KeyId", "KeyType", Event.EventId);
            return View(Event);
        }

        //
        // GET: /Event/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Event Event = db.Events.Find(id);
            if (Event == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventCreatedBy = new SelectList(db.Users, "UserId", "UserFirstName", Event.EventCreatedBy);
            ViewBag.EventClubId = new SelectList(db.Clubs, "ClubId", "ClubName", Event.EventClubId);
            ViewBag.EventId = new SelectList(db.Keys, "KeyId", "KeyType", Event.EventId);
            return View(Event);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        public ActionResult Edit(Event Event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventCreatedBy = new SelectList(db.Users, "UserId", "UserFirstName", Event.EventCreatedBy);
            ViewBag.EventClubId = new SelectList(db.Clubs, "ClubId", "ClubName", Event.EventClubId);
            ViewBag.EventId = new SelectList(db.Keys, "KeyId", "KeyType", Event.EventId);
            return View(Event);
        }

        //
        // GET: /Event/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Event Event = db.Events.Find(id);
            if (Event == null)
            {
                return HttpNotFound();
            }
            return View(Event);
        }

        //
        // POST: /Event/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Event Event = db.Events.Find(id);
            db.Events.Remove(Event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // POST: /EventAttending/Create

        [HttpPost]
        public ActionResult EventAttending(int eventId, int userId, int eventValue)
        {
            EventAttending eventattending = new EventAttending();
            eventattending.EventAttendingEventId = eventId;
            eventattending.EventAttendingUserId = userId;
            eventattending.EventAttendingValue = eventValue;
            var flag = false;
            NCRSNEntity tempDb = new NCRSNEntity();
            if (tempDb.EventAttendings.Where(e => e.EventAttendingEventId == eventattending.EventAttendingEventId && e.EventAttendingUserId == eventattending.EventAttendingUserId).Count() > 0)
            {
                flag = true;
                eventattending.EventAttendingId = tempDb.EventAttendings.Where(e => e.EventAttendingEventId == eventattending.EventAttendingEventId && e.EventAttendingUserId == eventattending.EventAttendingUserId).First().EventAttendingId;
            }
            tempDb.Dispose();
            if (ModelState.IsValid)
            {
                if (flag)
                {
                    db.Entry(eventattending).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    db.EventAttendings.Add(eventattending);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                
                }
            }

            ViewBag.EventAttendingEventId = new SelectList(db.Events, "EventId", "EventName", eventattending.EventAttendingEventId);
            ViewBag.EventAttendingUserId = new SelectList(db.Users, "UserId", "UserFirstName", eventattending.EventAttendingUserId);
            return View(eventattending);
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}