using EventApp.EventApp;
using EventApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace EventApp.Controllers
{
    // Bunun için bir Model oluşturmadım, içerisinde Link'lerden başka birşey olmayacak diye
    public class HomeController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult Home()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                User LoggedUser = (User)Session["LoggedIn"];

                var AllEvents = db.Event.ToList();
                var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
                ViewData["EventName"] = IDofEvent[0].Name;
                Event ThisEvent = db.Event.Find(IDofEvent[0].Id);
                ViewData["Event"] = ThisEvent;

                SponsorList(IDofEvent[0].Id);
                ViewData["SavedCategoriesByOrder"] = TempData["SavedCategoriesByOrder"];
                ViewData["PartnerEventList"] = TempData["PartnerEventList"];
                ViewData["PartnerList"] = TempData["PartnerList"];

                // Speaker'ları alıyor
                var AllUserList = db.UserEvent.ToList();
                var UserOfEvent = new List<User>();
                for (int j = 0; j < AllUserList.Count; j++)
                {
                    if (AllUserList[j].EventID == IDofEvent[0].Id)
                    {
                        User newuserevent = db.User.Find(AllUserList[j].UserID);
                        UserOfEvent.Add(newuserevent);
                    }
                }

                List<User> EventsSpeakers = new List<User>();
                var TypeOfUser = db.UserType.ToList();
                var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Konuşmacı").ToList();
                for (int i = 0; i < UserOfEvent.Count; i++)
                {
                    if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                    {
                        User newuser = db.User.Find(UserOfEvent[i].Id);
                        EventsSpeakers.Add(newuser);
                    }
                }

                ViewData["SpeakerList"] = SelectRandomSpeaker(EventsSpeakers);

                return View();
            }
        }

        private static List<User> SelectRandomSpeaker(List<User> EventsSpeakers)
        {
            List<User> SpeakerList = new List<User>();
            int CountSpeaker = EventsSpeakers.Count;
            if (CountSpeaker < 4)
            {
                SpeakerList = EventsSpeakers;
            }
            else if (CountSpeaker == 4)
            {
                SpeakerList = EventsSpeakers.OrderBy(d => (d.Name + d.Surname)).ToList();
            }
            else if (CountSpeaker == 5)
            {
                Random rnd = new Random();
                int Rand = rnd.Next(0, 5);
                EventsSpeakers.Remove(EventsSpeakers[Rand]);
                SpeakerList = EventsSpeakers;
            }
            else if (CountSpeaker == 6)
            {
                Random rdd = new Random();
                int RandSpeaker1 = rdd.Next(0, 6);
                int RandSpeaker2 = rdd.Next(0, 6);
                if (RandSpeaker1 != RandSpeaker2)
                {
                    EventsSpeakers.Remove(EventsSpeakers[RandSpeaker1]);
                    EventsSpeakers.Remove(EventsSpeakers[RandSpeaker2]);
                    SpeakerList = EventsSpeakers;
                }
                else
                {
                    RandSpeaker1 = (RandSpeaker1 + 1) % 6;
                    EventsSpeakers.Remove(EventsSpeakers[RandSpeaker1]);
                    EventsSpeakers.Remove(EventsSpeakers[RandSpeaker2]);
                    SpeakerList = EventsSpeakers;
                }
            }
            else
            {
                Random rd = new Random();
                List<int> RandSpeaker = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int addthis = rd.Next(0, CountSpeaker);
                    RandSpeaker.Add(addthis);
                    for (int j = 0; j < RandSpeaker.Count -1; j++)
                    {
                        if (RandSpeaker[i] == RandSpeaker[j])
                        {
                            RandSpeaker.Remove(RandSpeaker[i]);
                            i--;
                        }
                    }
                }
                for (int t = 0; t < 4; t++)
                {
                    User AddThis = EventsSpeakers[RandSpeaker[t]];
                    SpeakerList.Add(AddThis);
                }
            }

            return new List<User>(SpeakerList);
        }

        private dynamic SponsorList(Guid EventID)
        {
            var AllCategories = db.PartnerEventCategory.ToList();
            var ThisCategories = AllCategories.Where(k => k.EventID == EventID).ToList();
            List<PartnerEventCategory> SavedCategories = new List<PartnerEventCategory>();
            for (int i = 0; i < ThisCategories.Count; i++)
            {
                PartnerEventCategory addthis = db.PartnerEventCategory.Find(ThisCategories[i].Id);
                SavedCategories.Add(addthis);
            }

            var SortedOrder = SavedCategories.OrderByDescending(q => q.Rating).ToList();
            List<PartnerEventCategory> SavedCategoriesByOrder = new List<PartnerEventCategory>();
            for (int i = SortedOrder.Count-1; i >= 0; i--)
            {
                PartnerEventCategory addthis = db.PartnerEventCategory.Find(SortedOrder[i].Id);
                SavedCategoriesByOrder.Add(addthis);
            }
            TempData["SavedCategoriesByOrder"] = SavedCategoriesByOrder;

            var AllPartnerEvents = db.PartnerEvent.ToList();
            var thisPartnerEvents = AllPartnerEvents.Where(l => l.EventID == EventID).ToList();
            List<PartnerEvent> PartnerEventList = new List<PartnerEvent>();
            for (int i = 0; i < thisPartnerEvents.Count; i++)
            {
                PartnerEvent AddThis = db.PartnerEvent.Find(thisPartnerEvents[i].Id);
                PartnerEventList.Add(AddThis);
            }
            TempData["PartnerEventList"] = PartnerEventList;

            List<Partner> PartnerList = new List<Partner>();
            for (int j = 0; j < PartnerEventList.Count; j++)
            {
                Partner AddThis = db.Partner.Find(PartnerEventList[j].PartnerID);
                PartnerList.Add(AddThis);
            }
            TempData["PartnerList"] = PartnerList;

            return 123;
        }

        public ActionResult Partners()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                User LoggedUser = (User)Session["LoggedIn"];

                var AllEvents = db.Event.ToList();
                var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
                ViewData["EventName"] = IDofEvent[0].Name;

                SponsorList(IDofEvent[0].Id);
                ViewData["SavedCategoriesByOrder"] = TempData["SavedCategoriesByOrder"];
                ViewData["PartnerEventList"] = TempData["PartnerEventList"];
                ViewData["PartnerList"] = TempData["PartnerList"];

                // Speaker'ları alıyor
                var AllUserList = db.UserEvent.ToList();
                var UserOfEvent = new List<User>();
                for (int j = 0; j < AllUserList.Count; j++)
                {
                    if (AllUserList[j].EventID == IDofEvent[0].Id)
                    {
                        User newuserevent = db.User.Find(AllUserList[j].UserID);
                        UserOfEvent.Add(newuserevent);
                    }
                }

                List<User> EventsSpeakers = new List<User>();
                var TypeOfUser = db.UserType.ToList();
                var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Konuşmacı").ToList();
                for (int i = 0; i < UserOfEvent.Count; i++)
                {
                    if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                    {
                        User newuser = db.User.Find(UserOfEvent[i].Id);
                        EventsSpeakers.Add(newuser);
                    }
                }

                ViewData["SpeakerList"] = SelectRandomSpeaker(EventsSpeakers);

                return View();
            }
        }
    }
}