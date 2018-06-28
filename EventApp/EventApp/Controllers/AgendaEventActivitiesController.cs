using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventApp.EventApp;
using EventApp.Models;
using System.Globalization;

namespace EventApp.Controllers
{
    public class AgendaEventActivitiesController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult List(string disp)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User LoggedUser = (User)Session["LoggedIn"];
            ViewData["UserName"] = LoggedUser.Name + " " + LoggedUser.Surname;

            string display = "Show All";
            if (disp == "My Schedule")
            {
                display = "My Schedule";
                MyList();
            }
            else
            {
                ShowAll();
            }

            string isValid = "Valid";
            List<Activity> AgendaList = (List<Activity>)TempData["AgendaList"];
            if (AgendaList.Count == 0)
            {
                isValid = "Not Valid";
            }

            ViewData["isValid"] = isValid;
            ViewData["Display"] = display;
            ViewData["DateNumber"] = TempData["DateNumber"];
            ViewData["Dates"] = TempData["Dates"];
            ViewData["SameDays"] = TempData["SameDays"];

            ViewData["AgendaList"] = AgendaList;
            ViewData["IsScheduled"] = (List<bool>)TempData["IsScheduled"];
            ViewData["CountScheduled"] = TempData["CountScheduled"];
            ViewData["TimeInterval"] = (List<string>)TempData["TimeInterval"];

            return View();
        }

        public double SortingActivityTime(String StartTime)
        {
            string sdate = StartTime.Replace(" January ", "-01-").Replace(" February ", "-02-").Replace(" March ", "-03-").Replace(" April ", "-04-").Replace(" May ", "-05-").Replace(" June ", "-06-").Replace(" July ", "-07-").Replace(" August ", "-08-").Replace(" September ", "-09-").Replace(" October ", "-10-").Replace(" November ", "-11-").Replace(" December ", "-12-");
            DateTime startdate = DateTime.ParseExact(sdate, "dd-MM-yyyy - HH:mm", new CultureInfo("tr"));
            TimeSpan span = (startdate - new DateTime(2016, 1, 1, 0, 0, 0));

            return (double)(span.TotalMinutes);
        }

        public ActionResult DetailsFromAgenda(Guid? IDofActivity)
        {
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();

            var ActivityDetails = db.Activity.ToList();
            var ThisActivity = ActivityDetails.Where(m => m.Id == IDofActivity).ToList();

            if (ThisActivity[0].ActivityCategoryID == ConferenceType[0].Id)
            {
                return RedirectToAction("ConferenceDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (ThisActivity[0].ActivityCategoryID == PresentationType[0].Id)
            {
                return RedirectToAction("PresentationDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (ThisActivity[0].ActivityCategoryID == PanelType[0].Id)
            {
                return RedirectToAction("PanelDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            return View();
        }

        public ActionResult AddtoMySchedule (Guid? IDofActivity, string display)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEVentActivities = db.EventActivity.ToList();
            var ThisActivity = AllEVentActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            UserEventActivity AddSchedule = new UserEventActivity();
            AddSchedule.AttendDay = DateTime.Now.ToLongDateString();
            AddSchedule.AttendTime = DateTime.Now.ToLongTimeString();
            AddSchedule.UserEventID = thisUserEvent[0].Id;
            AddSchedule.IsActive = true;
            AddSchedule.EventActivityID = ThisActivity[0].Id;

            db.UserEventActivity.Add(AddSchedule);
            db.SaveChanges();

            int MyAgendaCount = MyAgendaU(LoggedUser);
            Session["AgendaCountU"] = MyAgendaCount;
            return RedirectToAction("List", "AgendaEventActivities",  new { disp = display});
        }

        public ActionResult CancelActivitityFromMyList (Guid? IDofActivity, string display)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEventActivities = db.EventActivity.ToList();
            var ThisActivity = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            var UserActivityEventList = db.UserEventActivity.ToList();
            var CancelSchedule = UserActivityEventList.Where(l => l.EventActivityID == ThisActivity[0].Id && l.UserEventID == thisUserEvent[0].Id).ToList();
            UserEventActivity CancelThisActivity = db.UserEventActivity.Find(CancelSchedule[0].Id);

            db.UserEventActivity.Remove(CancelThisActivity);
            db.SaveChanges();

            int MyAgendaCount = MyAgendaU(LoggedUser);
            Session["AgendaCountU"] = MyAgendaCount;
            return RedirectToAction("List", "AgendaEventActivities", new { disp = display });
        }

        public dynamic IsScheduled (List<Activity> AgendaList)
        {
            // UserEventID
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID for all list

            var AllEventActivities = db.EventActivity.ToList();
            List<bool> Scheduled = new List<bool>();
            var UserActivityEventList = db.UserEventActivity.ToList();
            int countScheduled = 0;
            for (int i = 0; i < AgendaList.Count; i++)
            {
                var ThisActivity = AllEventActivities.Where(k => k.ActivityID == AgendaList[i].Id).ToList();
                var ScheduledOrNot = UserActivityEventList.Where(l => l.EventActivityID == ThisActivity[0].Id && l.UserEventID == thisUserEvent[0].Id).ToList();
                if (ScheduledOrNot.Count == 0)
                {
                    Scheduled.Add(false);
                }
                else
                {
                    countScheduled++;
                    Scheduled.Add(true);
                }
            }

            TempData["CountScheduled"] = countScheduled;

            return Scheduled;
        }

        public dynamic StartEndTimes(List<Activity> AgendaList)
        {
            List<string> startendtimes = new List<string>();
            for (int i = 0; i < AgendaList.Count; i++)
            {
                string dateonly = AgendaList[i].StartDateTime.ToString();
                string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);

                string[] hours = day[1].Split(new[] { ":" }, StringSplitOptions.None);
                if (day[2] == "PM")
                {
                    hours[0] = "*" + hours[0] + "*";
                    hours[0] = hours[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                }
                else if (day[2] == "AM")
                {
                    hours[0] = hours[0].Replace("12", "0");
                }
                string starttime = hours[0] + ":" + hours[1];

                string dateonlyend = AgendaList[i].EndDateTime.ToString();
                string[] dayend = dateonlyend.Split(new[] { " " }, StringSplitOptions.None);

                string[] hours2 = dayend[1].Split(new[] { ":" }, StringSplitOptions.None);
                if (dayend[2] == "PM")
                {
                    hours2[0] = "*" + hours2[0] + "*";
                    hours2[0] = hours2[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                }
                else if (dayend[2] == "AM")
                {
                    hours2[0] = hours2[0].Replace("12", "0");
                }

                string endtime = hours2[0] + ":" + hours2[1];

                string timeinterval = starttime + " - " + endtime;
                startendtimes.Add(timeinterval);
            }
            return startendtimes;
        }

        public dynamic MyList()
        {
            // UserEventID
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID
            var AllEventActivity = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivity.Count(); i++)
            {
                if (AllEventActivity[i].EventID == thisEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllEventActivity[i].Id);
                    ThisEventActivities.Add(toAgenda2);
                }
            }

            // From UserEventActivity
            List<EventActivity> ScheduledEventActivities = new List<EventActivity>();
            var AllScheduledActivities = db.UserEventActivity.ToList();
            for (int j = 0; j < ThisEventActivities.Count; j++)
            {
                var isscheduled = AllScheduledActivities.Where(k => k.EventActivityID == ThisEventActivities[j].Id && k.UserEventID == thisUserEvent[0].Id).ToList();
                if (isscheduled.Count == 1)
                {
                    EventActivity addthis = db.EventActivity.Find(ThisEventActivities[j].Id);
                    ScheduledEventActivities.Add(addthis);
                }
            }

            // ScheduledActivities
            var AllActivities = db.Activity.ToList();
            List<Activity> ActivitiesForAgenda = new List<Activity>();
            for (int t = 0; t < ScheduledEventActivities.Count; t++)
            {
                Activity addActivity = db.Activity.Find(ScheduledEventActivities[t].ActivityID);
                ActivitiesForAgenda.Add(addActivity);
            }

            // Conference Type'ları alıyoruz
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
            var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivitiesForAgenda.Count;
            List<Activity> AgendaList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivitiesForAgenda[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newconference.StartDate);
                    newconference.StartDate = unixTime.ToString();
                    newconference.EndDate = "Konferans";
                    AgendaList.Add(newconference);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Sunum";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Kahve Molası-Yemek-Parti";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Panel";
                    AgendaList.Add(newpresentation);
                }
            }

            List<Activity> SortedAgendaList = new List<Activity>();
            SortedAgendaList = AgendaList.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();

            List<string> dates = new List<string>();
            for (int i = 0; i < SortedAgendaList.Count; i++)
            {
                string dateonly = SortedAgendaList[i].StartDateTime.ToString();
                string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);
                string[] day2 = day[0].Split(new[] { "/" }, StringSplitOptions.None);
                string day3 = day2[1] + "-" + day2[0] + "-" + day2[2];
                string dateformat = day3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");
                dates.Add(dateformat);
            }

            int datenumber = 1;
            List<int> samelist = new List<int>();
            int count = 1;
            for (int k = 1; k < dates.Count; k++)
            {
                if (dates[k] != dates[k - 1])
                {
                    samelist.Add(count);
                    count = 1;
                    datenumber++;
                }
                else
                {
                    count++;
                }
            }
            samelist.Add(count);

            TempData["DateNumber"] = datenumber;
            TempData["Dates"] = dates;
            TempData["SameDays"] = samelist;

            TempData["AgendaList"] = SortedAgendaList;
            List<bool> ScheduledOrNot = IsScheduled(SortedAgendaList);
            TempData["IsScheduled"] = ScheduledOrNot;
            TempData["CountScheduled"] = TempData["CountScheduled"];
            List<string> timeinterval = StartEndTimes(SortedAgendaList);
            TempData["TimeInterval"] = timeinterval;

            return "";
        }

        public dynamic ShowAll()
        {
            // Event'in ID'sini aldık
            User LoggedUser = (User)Session["LoggedIn"];
            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Agenda'da listelenecek EventActivity'leri aldık (Event ID'den bağımsız)
            var AllAgendaActivities = db.AgendaEventActivity.ToList();
            List<EventActivity> AllAgendaEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllAgendaActivities.Count; i++)
            {
                EventActivity toAgenda = db.EventActivity.Find(AllAgendaActivities[i].EventActivityID);
                AllAgendaEventActivities.Add(toAgenda);
            }

            // LoggedIn olunan EventActivity'leri alıyoruz
            List<EventActivity> LoggedInAgendaActivities = new List<EventActivity>();
            for (int j = 0; j < AllAgendaEventActivities.Count; j++)
            {
                if (AllAgendaEventActivities[j].EventID == IDofEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllAgendaActivities[j].EventActivityID);
                    LoggedInAgendaActivities.Add(toAgenda2);
                }
            }

            // EventActivity'den Activity'leri alıyoruz
            List<Activity> ActivitiesForAgenda = new List<Activity>();
            for (int p = 0; p < LoggedInAgendaActivities.Count; p++)
            {
                Activity addact = db.Activity.Find(LoggedInAgendaActivities[p].ActivityID);
                ActivitiesForAgenda.Add(addact);
            }

            // Conference Type'ları alıyoruz
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
            var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivitiesForAgenda.Count;
            List<Activity> AgendaList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivitiesForAgenda[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newconference.StartDate);
                    newconference.StartDate = unixTime.ToString();
                    newconference.EndDate = "Konferans";
                    AgendaList.Add(newconference);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Sunum";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Kahve Molası-Yemek-Parti";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Panel";
                    AgendaList.Add(newpresentation);
                }
            }

            List<Activity> SortedAgendaList = new List<Activity>();
            SortedAgendaList = AgendaList.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();

            List<string> dates = new List<string>();
            for (int i = 0; i < SortedAgendaList.Count; i++)
            {
                string dateonly = SortedAgendaList[i].StartDateTime.ToString();
                string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);
                string[] day2 = day[0].Split(new[] { "/" }, StringSplitOptions.None);
                string day3 = day2[1] + "-" + day2[0] + "-" + day2[2];
                string dateformat = day3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");
                dates.Add(dateformat);
            }

            int datenumber = 1;
            List<int> samelist = new List<int>();
            int count = 1;
            for (int k = 1; k < dates.Count; k++)
            {
                if (dates[k] != dates[k - 1])
                {
                    samelist.Add(count);
                    count = 1;
                    datenumber++;
                }
                else
                {
                    count++;
                }
            }
            samelist.Add(count);

            TempData["DateNumber"] = datenumber;
            TempData["Dates"] = dates;
            TempData["SameDays"] = samelist;

            TempData["AgendaList"] = SortedAgendaList;
            List<bool> ScheduledOrNot = IsScheduled(SortedAgendaList);
            TempData["IsScheduled"] = ScheduledOrNot;
            TempData["CountScheduled"] = TempData["CountScheduled"];
            List<string> timeinterval = StartEndTimes(SortedAgendaList);
            TempData["TimeInterval"] = timeinterval;

            return "";
        }

        public ActionResult AddFromSpeakerDetails(Guid? IDofActivity, Guid? Speaker)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEVentActivities = db.EventActivity.ToList();
            var ThisActivity = AllEVentActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            UserEventActivity AddSchedule = new UserEventActivity();
            AddSchedule.AttendDay = DateTime.Now.ToLongDateString();
            AddSchedule.AttendTime = DateTime.Now.ToLongTimeString();
            AddSchedule.UserEventID = thisUserEvent[0].Id;
            AddSchedule.IsActive = true;
            AddSchedule.EventActivityID = ThisActivity[0].Id;

            db.UserEventActivity.Add(AddSchedule);
            db.SaveChanges();

            int MyAgendaCount = MyAgendaU(LoggedUser);
            Session["AgendaCountU"] = MyAgendaCount;
            return RedirectToAction("SpeakerDetails", "Users", new { sprdet = Speaker });
        }

        public ActionResult CancelFromSpeakerDetails(Guid? IDofActivity, Guid? Speaker)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEventActivities = db.EventActivity.ToList();
            var ThisActivity = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            var UserActivityEventList = db.UserEventActivity.ToList();
            var CancelSchedule = UserActivityEventList.Where(l => l.EventActivityID == ThisActivity[0].Id && l.UserEventID == thisUserEvent[0].Id).ToList();
            UserEventActivity CancelThisActivity = db.UserEventActivity.Find(CancelSchedule[0].Id);

            db.UserEventActivity.Remove(CancelThisActivity);
            db.SaveChanges();

            int MyAgendaCount = MyAgendaU(LoggedUser);
            Session["AgendaCountU"] = MyAgendaCount;
            return RedirectToAction("SpeakerDetails", "Users", new { sprdet = Speaker });
        }

        public ActionResult AddFromActivityPage(Guid? IDofActivity)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEVentActivities = db.EventActivity.ToList();
            var ThisActivityEvent = AllEVentActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            UserEventActivity AddSchedule = new UserEventActivity();
            AddSchedule.AttendDay = DateTime.Now.ToLongDateString();
            AddSchedule.AttendTime = DateTime.Now.ToLongTimeString();
            AddSchedule.UserEventID = thisUserEvent[0].Id;
            AddSchedule.IsActive = true;
            AddSchedule.EventActivityID = ThisActivityEvent[0].Id;

            db.UserEventActivity.Add(AddSchedule);
            db.SaveChanges();

            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();

            var ActivityDetails = db.Activity.ToList();
            var ThisActivity = ActivityDetails.Where(m => m.Id == IDofActivity).ToList();

            if (ThisActivity[0].ActivityCategoryID == ConferenceType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("ConferenceDetails", "Activities", new { IDofActivity = IDofActivity});
            }
            else if (ThisActivity[0].ActivityCategoryID == PresentationType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("PresentationDetails", "Activities", new { IDofActivity = IDofActivity});
            }
            else if (ThisActivity[0].ActivityCategoryID == PanelType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("PanelDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            return RedirectToAction("", "");
        }

        public ActionResult CancelFromActivityPage(Guid? IDofActivity)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();
            var AllEventActivities = db.EventActivity.ToList();
            var ThisActivityEvent = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
            var UserActivityEventList = db.UserEventActivity.ToList();
            var CancelSchedule = UserActivityEventList.Where(l => l.EventActivityID == ThisActivityEvent[0].Id && l.UserEventID == thisUserEvent[0].Id).ToList();
            UserEventActivity CancelThisActivity = db.UserEventActivity.Find(CancelSchedule[0].Id);

            db.UserEventActivity.Remove(CancelThisActivity);
            db.SaveChanges();

            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();

            var ActivityDetails = db.Activity.ToList();
            var ThisActivity = ActivityDetails.Where(m => m.Id == IDofActivity).ToList();

            if (ThisActivity[0].ActivityCategoryID == ConferenceType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("ConferenceDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (ThisActivity[0].ActivityCategoryID == PresentationType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("PresentationDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (ThisActivity[0].ActivityCategoryID == PanelType[0].Id)
            {
                int MyAgendaCount = MyAgendaU(LoggedUser);
                Session["AgendaCountU"] = MyAgendaCount;
                return RedirectToAction("PanelDetails", "Activities", new { IDofActivity = IDofActivity });
            }
            return RedirectToAction("", "");
        }

        public dynamic MyAgendaU(User LoggedUser)
        {
            // UserEventID
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID
            var AllEventActivity = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivity.Count(); i++)
            {
                if (AllEventActivity[i].EventID == thisEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllEventActivity[i].Id);
                    ThisEventActivities.Add(toAgenda2);
                }
            }

            // From UserEventActivity
            List<EventActivity> ScheduledEventActivities = new List<EventActivity>();
            var AllScheduledActivities = db.UserEventActivity.ToList();
            for (int j = 0; j < ThisEventActivities.Count; j++)
            {
                var isscheduled = AllScheduledActivities.Where(k => k.EventActivityID == ThisEventActivities[j].Id && k.UserEventID == thisUserEvent[0].Id).ToList();
                if (isscheduled.Count == 1)
                {
                    EventActivity addthis = db.EventActivity.Find(ThisEventActivities[j].Id);
                    ScheduledEventActivities.Add(addthis);
                }
            }

            // ScheduledActivities
            var AllActivities = db.Activity.ToList();
            List<Activity> ActivitiesForAgenda = new List<Activity>();
            for (int t = 0; t < ScheduledEventActivities.Count; t++)
            {
                Activity addActivity = db.Activity.Find(ScheduledEventActivities[t].ActivityID);
                ActivitiesForAgenda.Add(addActivity);
            }

            // Conference Type'ları alıyoruz
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
            var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivitiesForAgenda.Count;
            List<Activity> AgendaList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivitiesForAgenda[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newconference);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
            }

            return (AgendaList.Count);
        }

        public ActionResult RespActList(string disp)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User LoggedUser = (User)Session["LoggedIn"];
            ViewData["UserName"] = LoggedUser.Name + " " + LoggedUser.Surname;

            ResponsinbleList();

            string isValid = "Valid";
            List<Activity> AgendaList = (List<Activity>)TempData["AgendaList"];
            if (AgendaList.Count == 0)
            {
                isValid = "Not Valid";
            }

            ViewData["isValid"] = isValid;
            ViewData["DateNumber"] = TempData["DateNumber"];
            ViewData["Dates"] = TempData["Dates"];
            ViewData["SameDays"] = TempData["SameDays"];
            ViewData["AllActivities"] = TempData["AllActivities"];

            ViewData["AgendaList"] = AgendaList;
            ViewData["TimeInterval"] = (List<string>)TempData["TimeInterval"];
            
            return View();
        }

        public dynamic ResponsinbleList()
        {
            // UserEventID
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID
            var AllEventActivity = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivity.Count(); i++)
            {
                if (AllEventActivity[i].EventID == thisEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllEventActivity[i].Id);
                    ThisEventActivities.Add(toAgenda2);
                }
            }

            // From UserEventActivity
            List<EventActivity> ScheduledEventActivities = new List<EventActivity>();
            var AllRespoEA = db.ResponsibleEventActivity.ToList();
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                var isResp = AllRespoEA.Where(k => k.EventActivityID == ThisEventActivities[i].Id && k.ResponsibleID == LoggedUser.Id).ToList();
                if (isResp.Count == 1)
                {
                    EventActivity EA = db.EventActivity.Find(isResp[0].EventActivityID);
                    ScheduledEventActivities.Add(EA);
                }
            }

            // ScheduledActivities
            var AllActivities = db.Activity.ToList();
            List<Activity> ActivitiesForAgenda = new List<Activity>();
            for (int t = 0; t < ScheduledEventActivities.Count; t++)
            {
                Activity addActivity = db.Activity.Find(ScheduledEventActivities[t].ActivityID);
                ActivitiesForAgenda.Add(addActivity);
            }

            // Conference Type'ları alıyoruz
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
            var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivitiesForAgenda.Count;
            List<Activity> AgendaList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivitiesForAgenda[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newconference.StartDate);
                    newconference.StartDate = unixTime.ToString();
                    newconference.EndDate = "Konferans";
                    AgendaList.Add(newconference);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Sunum";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Kahve Molası-Yemek-Parti";
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    double unixTime = SortingActivityTime(newpresentation.StartDate);
                    newpresentation.StartDate = unixTime.ToString();
                    newpresentation.EndDate = "Panel";
                    AgendaList.Add(newpresentation);
                }
            }

            List<Activity> SortedAgendaList = new List<Activity>();
            SortedAgendaList = AgendaList.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();

            List<string> dates = new List<string>();
            for (int i = 0; i < SortedAgendaList.Count; i++)
            {
                string dateonly = SortedAgendaList[i].StartDateTime.ToString();
                string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);
                string[] day2 = day[0].Split(new[] { "/" }, StringSplitOptions.None);
                string day3 = day2[1] + "-" + day2[0] + "-" + day2[2];
                string dateformat = day3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");
                dates.Add(dateformat);
            }

            int datenumber = 1;
            List<int> samelist = new List<int>();
            int count = 1;
            for (int k = 1; k < dates.Count; k++)
            {
                if (dates[k] != dates[k - 1])
                {
                    samelist.Add(count);
                    count = 1;
                    datenumber++;
                }
                else
                {
                    count++;
                }
            }
            samelist.Add(count);

            var AllAct = db.Activity.ToList();
            int countAgendaActivity = 0;
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                var isinagenda = AllAct.Where(k => k.Id == ThisEventActivities[i].ActivityID).ToList();
                if (isinagenda.Count == 1)
                {
                    countAgendaActivity = countAgendaActivity + 1;
                }
            }

            TempData["DateNumber"] = datenumber;
            TempData["Dates"] = dates;
            TempData["SameDays"] = samelist;
            TempData["AllActivities"] = countAgendaActivity;

            TempData["AgendaList"] = SortedAgendaList;
            //List<bool> ScheduledOrNot = IsScheduled(SortedAgendaList);
            //TempData["IsScheduled"] = ScheduledOrNot;
            //TempData["CountScheduled"] = TempData["CountScheduled"];
            List<string> timeinterval = StartEndTimes(SortedAgendaList);
            TempData["TimeInterval"] = timeinterval;

            return "";
        }

        public ActionResult Calendar()
        {
            return View();
        }


























































        // GET: AgendaEventActivities
        public ActionResult Index()
        {
            return View(db.AgendaEventActivity.ToList());
        }

        // GET: AgendaEventActivities/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaEventActivity agendaEventActivity = db.AgendaEventActivity.Find(id);
            if (agendaEventActivity == null)
            {
                return HttpNotFound();
            }
            return View(agendaEventActivity);
        }

        // GET: AgendaEventActivities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AgendaEventActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventActivityID")] AgendaEventActivity agendaEventActivity)
        {
            if (ModelState.IsValid)
            {
                agendaEventActivity.Id = Guid.NewGuid();
                db.AgendaEventActivity.Add(agendaEventActivity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(agendaEventActivity);
        }

        // GET: AgendaEventActivities/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaEventActivity agendaEventActivity = db.AgendaEventActivity.Find(id);
            if (agendaEventActivity == null)
            {
                return HttpNotFound();
            }
            return View(agendaEventActivity);
        }

        // POST: AgendaEventActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventActivityID")] AgendaEventActivity agendaEventActivity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agendaEventActivity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(agendaEventActivity);
        }

        // GET: AgendaEventActivities/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaEventActivity agendaEventActivity = db.AgendaEventActivity.Find(id);
            if (agendaEventActivity == null)
            {
                return HttpNotFound();
            }
            return View(agendaEventActivity);
        }

        // POST: AgendaEventActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AgendaEventActivity agendaEventActivity = db.AgendaEventActivity.Find(id);
            db.AgendaEventActivity.Remove(agendaEventActivity);
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
