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
    public class UsersController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult Attenders()
        {
            // Login olan User Bilgleri
            User LoggedUser = (User)Session["LoggedIn"];

            // Tüm Event'ler'den logged in olunan Event'i bulduk
            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Tüm Eventlerin User listesinden logged in olunan Event'in User listesini çektik
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

            // Event'in User'larından Attender'ları aldık
            List<User> EventsAttenders = new List<User>();
            var TypeOfUser = db.UserType.ToList();
            var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Katılımcı").ToList();
            for (int i = 0; i < UserOfEvent.Count; i++)
            {
                if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                {
                    User newuser = db.User.Find(UserOfEvent[i].Id);
                    EventsAttenders.Add(newuser);
                }
            }

            List<User> SortedAttenderList = new List<User>();
            SortedAttenderList = EventsAttenders.OrderBy(d => (d.Name + d.Surname)).ToList();

            ViewData["EventName"] = IDofEvent[0].Name;
            ViewData["AttenderList"] = SortedAttenderList;
            return View();
        }

        public ActionResult AttendersOrg()
        {
            // Login olan User Bilgleri
            User LoggedUser = (User)Session["LoggedIn"];

            // Tüm Event'ler'den logged in olunan Event'i bulduk
            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Tüm Eventlerin User listesinden logged in olunan Event'in User listesini çektik
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

            // Event'in User'larından Attender'ları aldık
            List<User> EventsAttenders = new List<User>();
            var TypeOfUser = db.UserType.ToList();
            var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Katılımcı").ToList();
            for (int i = 0; i < UserOfEvent.Count; i++)
            {
                if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                {
                    User newuser = db.User.Find(UserOfEvent[i].Id);
                    EventsAttenders.Add(newuser);
                }
            }

            List<User> SortedAttenderList = new List<User>();
            SortedAttenderList = EventsAttenders.OrderBy(d => (d.Name + d.Surname)).ToList();

            ViewData["EventName"] = IDofEvent[0].Name;
            ViewData["AttenderList"] = SortedAttenderList;
            return View();
        }

        public ActionResult Speakers()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

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

            List<User> SortedSpeakerList = new List<User>();
            SortedSpeakerList = EventsSpeakers.OrderBy(d => (d.Name + d.Surname)).ToList();

            Event ThisEvent = db.Event.Find(IDofEvent[0].Id);
            ViewData["EventName"] = ThisEvent.Name;

            ViewData["SpeakerList"] = SortedSpeakerList;
            return View();
        }

        public ActionResult SpeakersOrg()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

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

            List<User> SortedSpeakerList = new List<User>();
            SortedSpeakerList = EventsSpeakers.OrderBy(d => (d.Name + d.Surname)).ToList();

            Event ThisEvent = db.Event.Find(IDofEvent[0].Id);
            ViewData["EventName"] = ThisEvent.Name;

            ViewData["SpeakerList"] = SortedSpeakerList;
            return View();
        }

        public ActionResult AttenderDetails(Guid usrdet)
        {
            if (ModelState.IsValid)
            {
                AttenderPublishActivity(usrdet);
                ViewData["Activities"] = TempData["Activities"];
                ViewData["AttemptTimes"] = TempData["AttemptTimes"];
                ViewData["AttemptDay"] = TempData["AttemptDay"];
                ViewData["ActivityType"] = TempData["ActivityType"];
                User userinfo = db.User.Find(usrdet);
                ViewData["Attenderdetails"] = userinfo;
                return View();
            }
            else
            {
                return RedirectToAction("Attenders", "Users");
            }
        }

        public ActionResult AttenderDetailsOrg(Guid usrdet)
        {
            if (ModelState.IsValid)
            {
                AttenderPublishActivity(usrdet);
                ViewData["Activities"] = TempData["Activities"];
                ViewData["AttemptTimes"] = TempData["AttemptTimes"];
                ViewData["AttemptDay"] = TempData["AttemptDay"];
                ViewData["ActivityType"] = TempData["ActivityType"];
                User userinfo = db.User.Find(usrdet);
                ViewData["Attenderdetails"] = userinfo;
                return View();
            }
            else
            {
                return RedirectToAction("Attenders", "Users");
            }
        }

        public ActionResult SpeakerDetails(Guid sprdet)
        {
            if (ModelState.IsValid)
            {
                User speakerinfo = db.User.Find(sprdet);
                ViewData["SpeakerDetails"] = speakerinfo;

                var AllSpeakerEventActivities = db.SpeakerEventActivity.ToList();
                List<EventActivity> SpeakersAllEventActivities = new List<EventActivity>();
                for (int i = 0; i < AllSpeakerEventActivities.Count; i++)
                {
                    if (AllSpeakerEventActivities[i].SpeakerID == sprdet)
                    {
                        EventActivity addthis = db.EventActivity.Find(AllSpeakerEventActivities[i].EventActivityID);
                        SpeakersAllEventActivities.Add(addthis);
                    }
                }

                var isvalid = db.AgendaEventActivity.ToList();
                List<EventActivity> SpeakersEventActivities = new List<EventActivity>();
                for (int k = 0; k < SpeakersAllEventActivities.Count; k++)
                {
                    var check = isvalid.Where(t => t.EventActivityID == SpeakersAllEventActivities[k].Id).ToList();
                    if (check.Count == 1)
                    {
                        EventActivity add = db.EventActivity.Find(SpeakersAllEventActivities[k].Id);
                        SpeakersEventActivities.Add(add);
                    }
                }

                var AllActivities = db.Activity.ToList();
                List<Activity> SpeakersActivities = new List<Activity>();
                for (int j = 0; j < SpeakersEventActivities.Count; j++)
                {
                    Activity AddThis = db.Activity.Find(SpeakersEventActivities[j].ActivityID);
                    SpeakersActivities.Add(AddThis);
                }

                // Conference Type'ları alıyoruz
                var TypeOfActivity = db.ActivityCategory.ToList();
                var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
                var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
                var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();

                // Activity Name'ini buluyor
                int countActivityList = SpeakersActivities.Count;
                List<Activity> AgendaList = new List<Activity>();
                for (int i = 0; i < countActivityList; i++)
                {
                    if (SpeakersActivities[i].ActivityCategoryID == ConferenceType[0].Id)
                    {
                        Activity newconference = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newconference.StartDate);
                        newconference.StartDate = unixTime.ToString();
                        newconference.EndDate = "Konferans";
                        AgendaList.Add(newconference);
                    }
                    else if (SpeakersActivities[i].ActivityCategoryID == PresentationType[0].Id)
                    {
                        Activity newpresentation = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Sunum";
                        AgendaList.Add(newpresentation);
                    }
                    else if (SpeakersActivities[i].ActivityCategoryID == PanelType[0].Id)
                    {
                        Activity newpresentation = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Panel";
                        AgendaList.Add(newpresentation);
                    }
                }

                bool isExist = false;
                var AllActs = db.Activity.ToList();
                var PanelManager = AllActs.Where(k => k.EventOrEventActivityForSurveyAndDiscussion == sprdet).ToList();
                for (int i = 0; i < PanelManager.Count; i++)
                {
                    isExist = false;
                    for (int t = 0; t < AgendaList.Count; t++)
                    {

                        if (AgendaList[t].Id == PanelManager[i].Id)
                        {
                            isExist = true;
                        }
                    }
                    if (isExist == false)
                    {
                        Activity newpresentation = db.Activity.Find(PanelManager[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Panel";
                        AgendaList.Add(newpresentation);
                    }
                }

                List<Activity> SpeakerSessions = new List<Activity>();
                SpeakerSessions = AgendaList.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();
                // Yukarıda Agenda'ya dahil olan Speaker'ın Activity'lerini aldık

                List<bool> ScheduledOrNot = IsScheduled(SpeakerSessions);
                // Scheduled of not aldık bool olarak

                List<string> times = StartEndTimes(SpeakerSessions);
                ViewData["StartEndTimes"] = times;
                List<string> StartDate = (List<string>)TempData["Date"];
                ViewData["StartDate"] = StartDate;

                ViewData["SpeakerSessions"] = SpeakerSessions;
                ViewData["isScheduled"] = ScheduledOrNot;

                return View();
            }
            else
            {
                return RedirectToAction("Speakers", "Users");
            }
        }

        public ActionResult SpeakerDetailsOrg(Guid sprdet)
        {
            if (ModelState.IsValid)
            {
                User speakerinfo = db.User.Find(sprdet);
                ViewData["SpeakerDetails"] = speakerinfo;

                var AllSpeakerEventActivities = db.SpeakerEventActivity.ToList();
                List<EventActivity> SpeakersAllEventActivities = new List<EventActivity>();
                for (int i = 0; i < AllSpeakerEventActivities.Count; i++)
                {
                    if (AllSpeakerEventActivities[i].SpeakerID == sprdet)
                    {
                        EventActivity addthis = db.EventActivity.Find(AllSpeakerEventActivities[i].EventActivityID);
                        SpeakersAllEventActivities.Add(addthis);
                    }
                }

                var isvalid = db.AgendaEventActivity.ToList();
                List<EventActivity> SpeakersEventActivities = new List<EventActivity>();
                for (int k = 0; k < SpeakersAllEventActivities.Count; k++)
                {
                    var check = isvalid.Where(t => t.EventActivityID == SpeakersAllEventActivities[k].Id).ToList();
                    if (check.Count == 1)
                    {
                        EventActivity add = db.EventActivity.Find(SpeakersAllEventActivities[k].Id);
                        SpeakersEventActivities.Add(add);
                    }
                }

                var AllActivities = db.Activity.ToList();
                List<Activity> SpeakersActivities = new List<Activity>();
                for (int j = 0; j < SpeakersEventActivities.Count; j++)
                {
                    Activity AddThis = db.Activity.Find(SpeakersEventActivities[j].ActivityID);
                    SpeakersActivities.Add(AddThis);
                }

                // Conference Type'ları alıyoruz
                var TypeOfActivity = db.ActivityCategory.ToList();
                var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
                var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
                var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();

                // Activity Name'ini buluyor
                int countActivityList = SpeakersActivities.Count;
                List<Activity> AgendaList = new List<Activity>();
                for (int i = 0; i < countActivityList; i++)
                {
                    if (SpeakersActivities[i].ActivityCategoryID == ConferenceType[0].Id)
                    {
                        Activity newconference = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newconference.StartDate);
                        newconference.StartDate = unixTime.ToString();
                        newconference.EndDate = "Konferans";
                        AgendaList.Add(newconference);
                    }
                    else if (SpeakersActivities[i].ActivityCategoryID == PresentationType[0].Id)
                    {
                        Activity newpresentation = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Sunum";
                        AgendaList.Add(newpresentation);
                    }
                    else if (SpeakersActivities[i].ActivityCategoryID == PanelType[0].Id)
                    {
                        Activity newpresentation = db.Activity.Find(SpeakersActivities[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Panel";
                        AgendaList.Add(newpresentation);
                    }
                }

                bool isExist = false;
                var AllActs = db.Activity.ToList();
                var PanelManager = AllActs.Where(k => k.EventOrEventActivityForSurveyAndDiscussion == sprdet).ToList();
                for (int i = 0; i < PanelManager.Count; i++)
                {
                    isExist = false;
                    for (int t = 0; t < AgendaList.Count; t++)
                    {
                                                
                        if (AgendaList[t].Id == PanelManager[i].Id)
                        {
                            isExist = true;
                        }
                    }
                    if (isExist == false)
                    {
                        Activity newpresentation = db.Activity.Find(PanelManager[i].Id);
                        double unixTime = SortingActivityTime(newpresentation.StartDate);
                        newpresentation.StartDate = unixTime.ToString();
                        newpresentation.EndDate = "Panel";
                        AgendaList.Add(newpresentation);
                    } 
                }

                List<Activity> SpeakerSessions = new List<Activity>();
                SpeakerSessions = AgendaList.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();
                // Yukarıda Agenda'ya dahil olan Speaker'ın Activity'lerini aldık

                List<bool> ScheduledOrNot = IsScheduled(SpeakerSessions);
                // Scheduled of not aldık bool olarak

                List<string> times = StartEndTimes(SpeakerSessions);
                ViewData["StartEndTimes"] = times;
                List<string> StartDate = (List<string>)TempData["Date"];
                ViewData["StartDate"] = StartDate;

                ViewData["SpeakerSessions"] = SpeakerSessions;
                ViewData["isScheduled"] = ScheduledOrNot;

                return View();
            }
            else
            {
                return RedirectToAction("Speakers", "Users");
            }
        }

        public double SortingActivityTime(String StartTime)
        {
            string sdate = StartTime.Replace(" January ", "-01-").Replace(" February ", "-02-").Replace(" March ", "-03-").Replace(" April ", "-04-").Replace(" May ", "-05-").Replace(" June ", "-06-").Replace(" July ", "-07-").Replace(" August ", "-08-").Replace(" September ", "-09-").Replace(" October ", "-10-").Replace(" November ", "-11-").Replace(" December ", "-12-");
            DateTime startdate = DateTime.ParseExact(sdate, "dd-MM-yyyy - HH:mm", new CultureInfo("tr"));
            TimeSpan span = (startdate - new DateTime(2016, 1, 1, 0, 0, 0));

            return (double)(span.TotalMinutes);
        }

        public dynamic IsScheduled(List<Activity> AgendaList)
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
            List<string> dates = new List<string>();
            for (int i = 0; i < AgendaList.Count; i++)
            {
                string dateonly = AgendaList[i].StartDateTime.ToString();
                string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);
                string[] day2 = day[0].Split(new[] { "/" }, StringSplitOptions.None);
                string day3 = day2[1] + "-" + day2[0] + "-" + day2[2];
                string dateformat = day3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");
                dates.Add(dateformat);
            }

            TempData["Date"] = dates;

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

        public dynamic AttenderPublishActivity(Guid AttenderID)
        {
            // UserEvent
            User LoggedUser = db.User.Find(AttenderID);
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // UserEventActivity
            List<UserEventActivity> allusereventactivities = db.UserEventActivity.ToList();
            List<UserEventActivity> ScheduledUserEventActivities = new List<UserEventActivity>();
            var scheduled = allusereventactivities.Where(k => k.UserEventID == thisUserEvent[0].Id).ToList();
            for (int i = 0; i < scheduled.Count; i++)
            {
                UserEventActivity add = db.UserEventActivity.Find(scheduled[i].Id);

                UserEventActivity deneme = add;
                string day = deneme.AttendDay.Replace("January", "01,").Replace("February", "02,").Replace("March", "03,").Replace("April", "04,").Replace("May", "05,").Replace("June", "06,").Replace("July", "07,").Replace("August", "08,").Replace("September", "09,").Replace("October", "10,").Replace("November", "11,").Replace("December", "12,");
                string[] daydivide = day.Split(new[] { ", " }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[3];

                string[] timedivide = deneme.AttendTime.Split(new[] { " " }, StringSplitOptions.None);
                string hournormal = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    hournormal = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    hournormal = hourdivide[0] + ":" + hourdivide[1];
                }
                deneme.AttendDay = daynormal + " " + hournormal;
                DateTime dt = new DateTime();

                // .ToString("dd-MM-yyyy HH:mm")

                TimeSpan sss = (dt - new DateTime(2016, 1, 1, 0, 0, 0));
                deneme.AttendTime = sss.TotalMinutes.ToString();

                ScheduledUserEventActivities.Add(deneme);
            }

            List<UserEventActivity> SortedTime = new List<UserEventActivity>();
            SortedTime = ScheduledUserEventActivities.OrderBy(d => Convert.ToDouble(d.AttendTime)).ToList();

            List<Activity> ToDetailsPage = new List<Activity>();
            List<string> scheduleday = new List<string>();
            List<string> scheduletime = new List<string>();
            List<string> ActivityType = new List<string>();
            for (int h = 0; h < SortedTime.Count; h++)
            {
                var TypeOfActivity = db.ActivityCategory.ToList();
                var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
                var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
                var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
                var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();
               
                string[] datetime = SortedTime[h].AttendDay.Split(new[] { " " }, StringSplitOptions.None);
                string time = datetime[1];
                string[] ddd = datetime[0].Split(new[] { "-" }, StringSplitOptions.None);
                string month = ddd[1].Replace("01", "ocak").Replace("02", "şub").Replace("03", "mar").Replace("04", "nis").Replace("05", "may").Replace("06", "haz").Replace("07", "tem").Replace("08", "ağus").Replace("09", "eyl").Replace("10", "ekim").Replace("11", "kas").Replace("12", "ara");
                string day = month + " " + ddd[0];

                scheduleday.Add(day);
                scheduletime.Add(time);
                EventActivity addfrom = db.EventActivity.Find(SortedTime[h].EventActivityID);
                Activity addthis = db.Activity.Find(addfrom.ActivityID);

                if (addthis.ActivityCategoryID == ConferenceType[0].Id)
                {
                    ActivityType.Add("Konferans");
                }
                else if (addthis.ActivityCategoryID == PresentationType[0].Id)
                {
                    ActivityType.Add("Sunum");
                }
                else if (addthis.ActivityCategoryID == PanelType[0].Id)
                {
                    ActivityType.Add("Panel");
                }
                else if (addthis.ActivityCategoryID == CoffeeType[0].Id)
                {
                    ActivityType.Add("Kahve Molası-Yemek-Parti");
                }

                ToDetailsPage.Add(addthis);
            }

            TempData["ActivityType"] = ActivityType;
            TempData["Activities"] = ToDetailsPage;
            TempData["AttemptTimes"] = scheduletime;
            TempData["AttemptDay"] = scheduleday;

            return "";
        }

        public ActionResult EditAttender (Guid AttenderId)
        {
            User EditUser = db.User.Find(AttenderId);
            string NS = EditUser.Name + " " +  EditUser.Surname;
            ViewData["NameSurname"] = NS;
            ViewData["ChangeID"] = EditUser.Id;
            return View(EditUser);
        }

        public ActionResult EditSpeaker(Guid SpeakerId)
        {
            User EditUser = db.User.Find(SpeakerId);
            string NS = EditUser.Name + " " + EditUser.Surname;
            ViewData["NameSurname"] = NS;
            ViewData["ChangeID"] = EditUser.Id;
            return View(EditUser);
        }

        public ActionResult EditMyProfileU(Guid AttenderId)
        {
            User EditUser = db.User.Find(AttenderId);
            string NS = EditUser.Name + " " + EditUser.Surname;
            ViewData["NameSurname"] = NS;
            ViewData["ChangeID"] = EditUser.Id;
            return View(EditUser);
        }

        public ActionResult MyProfileU(Guid usrdet)
        {
            if (ModelState.IsValid)
            {
                AttenderPublishActivity(usrdet);
                ViewData["Activities"] = TempData["Activities"];
                ViewData["AttemptTimes"] = TempData["AttemptTimes"];
                ViewData["AttemptDay"] = TempData["AttemptDay"];
                ViewData["ActivityType"] = TempData["ActivityType"];
                User userinfo = db.User.Find(usrdet);
                ViewData["Attenderdetails"] = userinfo;
                return View();
            }
            else
            {
                return RedirectToAction("Attenders", "Users");
            }
        }

        public ActionResult Notifications()
        {
            return View();
        }






















        // GET: Users
        public ActionResult Index()
        {
            return View(db.User.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserTypeID,Name,Surname,Mail,Phone,Company,PersonelTitle,CountryPhoneCode,City,About,PhotoDirectory,CheckIn,Facebook,Twitter,Instagram,Linkedin,Website,Password,ConfirmPassword,AccountConfirmation,IsActive,RoomType,NationalIdentityNumber,CheckInDate,CheckOutDate")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserTypeID,Name,Surname,Mail,Phone,Company,PersonelTitle,CountryPhoneCode,City,About,PhotoDirectory,CheckIn,Facebook,Twitter,Instagram,Linkedin,Website,Password,ConfirmPassword,AccountConfirmation,IsActive,RoomType,NationalIdentityNumber,CheckInDate,CheckOutDate")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
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
