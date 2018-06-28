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
    public class ActivitiesController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult DeleteDocument(Guid docID, Guid IDofActivity, string returnto)
        {
            Guid DeleteID = docID;
            Document doc = db.Document.Find(docID);
            db.Document.Remove(doc);
            db.SaveChanges();

            var AllDocumentEvents = db.DocumentEventActivity.ToList();
            var DeleteDoc = AllDocumentEvents.Where(k => k.DocumentID == DeleteID).ToList();

            for (int i = 0; i < DeleteDoc.Count; i++)
            {
                DocumentEventActivity DocEvent = db.DocumentEventActivity.Find(DeleteDoc[i].Id);
                db.DocumentEventActivity.Remove(DocEvent);
                db.SaveChanges();
            }

            if (returnto == "ConferenceDetailsEditable")
            {
                // EventActivity'i aldık
                var AllEventActivities = db.EventActivity.ToList();
                var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

                // AddedSpeakers
                var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                List<User> AddedSpeaker = new List<User>();
                for (int i = 0; i < SpeakerList.Count; i++)
                {
                    User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                    AddedSpeaker.Add(speaker);
                }

                // AddedResponsibles
                var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                List<User> AddedResponsible = new List<User>();
                for (int i = 0; i < ResponsibleList.Count; i++)
                {
                    User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                    AddedResponsible.Add(responsible);
                }

                // AddedPlaces
                var AllAddedPlaceList = db.MapEventActivity.ToList();
                var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
                List<Map> AddedPlaces = new List<Map>();
                for (int i = 0; i < PlaceList.Count; i++)
                {
                    Map place = db.Map.Find(PlaceList[i].MapID);
                    AddedPlaces.Add(place);
                }

                Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

                TempData["Model"] = act;
                TempData["AddedSpeaker"] = AddedSpeaker;
                TempData["AddedResponsible"] = AddedResponsible;
                TempData["AddedPlaces"] = AddedPlaces;
                return RedirectToAction("ConferenceDetailsEditable", "Activities");
            }
            else if (returnto == "ConferenceDetailsOrg")
            {
                return RedirectToAction("ConferenceDetailsOrg", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (returnto == "PresentationDetailsOrg")
            {
                return RedirectToAction("PresentationDetailsOrg", "Activities", new { IDofActivity = IDofActivity });
            }
            else if (returnto == "PresentationDetailsEditable")
            {
                // EventActivity'i aldık
                var AllEventActivities = db.EventActivity.ToList();
                var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

                // AddedSpeakers
                var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                List<User> AddedSpeaker = new List<User>();
                for (int i = 0; i < SpeakerList.Count; i++)
                {
                    User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                    AddedSpeaker.Add(speaker);
                }

                // AddedResponsibles
                var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                List<User> AddedResponsible = new List<User>();
                for (int i = 0; i < ResponsibleList.Count; i++)
                {
                    User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                    AddedResponsible.Add(responsible);
                }

                // AddedPlaces
                var AllAddedPlaceList = db.MapEventActivity.ToList();
                var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
                List<Map> AddedPlaces = new List<Map>();
                for (int i = 0; i < PlaceList.Count; i++)
                {
                    Map place = db.Map.Find(PlaceList[i].MapID);
                    AddedPlaces.Add(place);
                }

                Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

                TempData["Model"] = act;
                TempData["AddedSpeaker"] = AddedSpeaker;
                TempData["AddedResponsible"] = AddedResponsible;
                TempData["AddedPlaces"] = AddedPlaces;
                return RedirectToAction("PresentationDetailsEditable", "Activities");
            }

            return RedirectToAction("AddDocument", "Documents");
        }

        public ActionResult SelectActivityCategory()
        {
            List<ActivityCategory> ActivityCategoryList = new List<ActivityCategory>();
            List<ActivityCategory> CategoryList = db.ActivityCategory.ToList();

            ActivityCategoryList = CategoryList.OrderBy(d => (d.Name)).ToList();

            return View(ActivityCategoryList);
        }

        public dynamic StartEndTimes(Activity act)
        {
            string dateonly = act.StartDateTime.ToString();
            string[] day = dateonly.Split(new[] { " " }, StringSplitOptions.None);
            string[] day2 = day[0].Split(new[] { "/" }, StringSplitOptions.None);
            string day3 = day2[1] + "-" + day2[0] + "-" + day2[2];
            string sdate = day3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");

            string[] hour = dateonly.Split(new[] { " " }, StringSplitOptions.None);

            string[] hours = hour[1].Split(new[] { ":" }, StringSplitOptions.None);
            if (day[2] == "PM")
            {
                hours[0] = "*" + hours[0] + "*";
                hours[0] = hours[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
            }
            else if (day[2] == "AM")
            {
                hours[0] = hours[0].Replace("12", "0");
            }
            string stime = hours[0] + ":" + hours[1];

            string dateonly2 = act.EndDateTime.ToString();
            string[] dayend = dateonly2.Split(new[] { " " }, StringSplitOptions.None);
            string[] dayend2 = dayend[0].Split(new[] { "/" }, StringSplitOptions.None);
            string dayend3 = dayend2[1] + "-" + dayend2[0] + "-" + dayend2[2];
            string edate = dayend3.Replace("-1-", " Ocak ").Replace("-2-", " Şubat ").Replace("-3-", " Mart ").Replace("-4-", " Nisan ").Replace("-5-", " Mayıs ").Replace("-6-", " Haziran ").Replace("-7-", " Temmuz ").Replace("-8-", " Ağustos ").Replace("-9-", " Eylül ").Replace("-10-", " Ekim ").Replace("-11-", " Kasım ").Replace("-12-", " Aralık ");

            string[] hourend = dateonly2.Split(new[] { " " }, StringSplitOptions.None);

            string[] hoursend = hourend[1].Split(new[] { ":" }, StringSplitOptions.None);
            if (dayend[2] == "PM")
            {
                hoursend[0] = "*" + hoursend[0] + "*";
                hoursend[0] = hoursend[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
            }
            else if (dayend[2] == "AM")
            {
                hoursend[0] = hoursend[0].Replace("12", "0");
            }
            string etime = hoursend[0] + ":" + hoursend[1];

            string start = sdate + " - " + stime;
            string end = edate + " - " + etime;

            List<string> dates = new List<string>();
            dates.Add(start);
            dates.Add(end);
            return dates;
        }

        public ActionResult ActivityListEditable()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Bu Event'in Activity'lerini aldık
            var AllEventActivities = db.EventActivity.ToList();
            var ThisEventsActivities = AllEventActivities.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            // Yukarıda alınan Activity ID'lerinden Activity tablosuna gidip Activity bilgilerini alıyoruz
            var AllActivities = db.Activity.ToList();
            List<Activity> ActivityDetails = new List<Activity>();
            for (int i = 0; i < ThisEventsActivities.Count; i++)
            {
                Activity details = db.Activity.Find(ThisEventsActivities[i].ActivityID);
                ActivityDetails.Add(details);
            }

            //Panel
            var PanelTypeOfActivity = db.ActivityCategory.ToList();
            var PanelType = PanelTypeOfActivity.Where(k => k.Name == "Panel").ToList();

            // Activity Name'ini buluyor
            int PanelcountActivityList = ActivityDetails.Count;
            List<Activity> PanelList = new List<Activity>();
            for (int i = 0; i < PanelcountActivityList; i++)
            {
                if (ActivityDetails[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpanel = db.Activity.Find(ActivityDetails[i].Id);
                    PanelList.Add(newpanel);
                }
            }

            List<Activity> SortedPanelList = new List<Activity>();
            SortedPanelList = PanelList.OrderByDescending(d => d.Name).ToList();

            ViewData["PanelList"] = SortedPanelList;
            ViewData["PanelTypeID"] = PanelType[0].Id;


            // Buradan sonrasını diğer ActivityCategory'ler için de yapmak lazım
            // ConferenceCategory'nin ID'sini aldık
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivityDetails.Count;
            List<Activity> ConferenceList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivityDetails[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivityDetails[i].Id);
                    ConferenceList.Add(newconference);
                }
            }

            List<Activity> SortedConferenceList = new List<Activity>();
            SortedConferenceList = ConferenceList.OrderByDescending(d => d.Name).ToList();

            ViewData["ConferenceList"] = SortedConferenceList;
            ViewData["ConferenceTypeID"] = ConferenceType[0].Id;

            // Presentation
            var PreTypeOfActivity = db.ActivityCategory.ToList();
            var PresentationType = PreTypeOfActivity.Where(k => k.Name == "Sunum").ToList();

            // Activity Name'ini buluyor
            int PrecountActivityList = ActivityDetails.Count;
            List<Activity> PresentationList = new List<Activity>();
            for (int i = 0; i < PrecountActivityList; i++)
            {
                if (ActivityDetails[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivityDetails[i].Id);
                    PresentationList.Add(newpresentation);
                }
            }

            List<Activity> SortedPresentationList = new List<Activity>();
            SortedPresentationList = PresentationList.OrderByDescending(d => d.Name).ToList();

            ViewData["PresentationList"] = SortedPresentationList;
            ViewData["PresentationTypeID"] = PresentationType[0].Id;

            // Coffee
            var CoffeeTypeOfActivity = db.ActivityCategory.ToList();
            var CoffeeType = CoffeeTypeOfActivity.Where(k => k.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int CoffeecountActivityList = ActivityDetails.Count;
            List<Activity> CoffeeList = new List<Activity>();
            for (int i = 0; i < CoffeecountActivityList; i++)
            {
                if (ActivityDetails[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpcoffee = db.Activity.Find(ActivityDetails[i].Id);
                    CoffeeList.Add(newpcoffee);
                }
            }

            List<Activity> SortedCoffeeList = new List<Activity>();
            SortedCoffeeList = CoffeeList.OrderByDescending(d => d.Name).ToList();

            ViewData["CoffeeList"] = SortedCoffeeList;
            ViewData["CoffeeTypeID"] = CoffeeType[0].Id;

            

            return View();
        }

        public ActionResult Routing(Guid ActivityCategory)
        {
            // Activity Name'ini buluyor
            List<ActivityCategory> CategoryList = db.ActivityCategory.ToList();
            ActivityCategory NewActivity = new ActivityCategory();
            int count = CategoryList.Count;
            for (int i = 0; i < count; i++)
            {
                if (ActivityCategory == CategoryList[i].Id)
                {
                    NewActivity.Id = CategoryList[i].Id;
                    NewActivity.Name = CategoryList[i].Name;
                }
            }

            if (NewActivity.Name == "Konferans")
            {
                string newconference = "newconference";
                List<User> AddedSpeakers = new List<User>();
                List<User> AddedResponsibles = new List<User>();
                List<Map> AddedPlaces = new List<Map>();
                Guid DeleteAfterEdit = Guid.Empty;
                Session["BackButton"] = "Create";
                return RedirectToAction("CreateConference", new { DeleteAfterEdit = DeleteAfterEdit, IsNew = newconference, AddedSpeakers = AddedSpeakers, AddedResponsibles = AddedResponsibles, AddedPlaces = AddedPlaces });
            }
            else if (NewActivity.Name == "Sunum")
            {
                string newpresentation = "newpresentation";
                List<User> AddedSpeakers = new List<User>();
                List<User> AddedResponsibles = new List<User>();
                List<Map> AddedPlaces = new List<Map>();
                Guid DeleteAfterEdit = Guid.Empty;
                Session["BackButton"] = "Create";
                return RedirectToAction("CreatePresentation", new { DeleteAfterEdit = DeleteAfterEdit, IsNew = newpresentation, AddedSpeakers = AddedSpeakers, AddedResponsibles = AddedResponsibles, AddedPlaces = AddedPlaces });
            }
            else if (NewActivity.Name == "Panel")
            {
                string newpresentation = "newpanel";
                List<User> AddedSpeakers = new List<User>();
                List<User> AddedResponsibles = new List<User>();
                List<Map> AddedPlaces = new List<Map>();
                Guid DeleteAfterEdit = Guid.Empty;
                Session["PanelBackButton"] = "Create";
                return RedirectToAction("CreatePanel", new { DeleteAfterEdit = DeleteAfterEdit, IsNew = newpresentation, AddedSpeakers = AddedSpeakers, AddedResponsibles = AddedResponsibles, AddedPlaces = AddedPlaces });
            }
            else if (NewActivity.Name == "Anket")
            {
                return RedirectToAction("CreateSurvey", "Activities");
            }
            else if (NewActivity.Name == "Kahve Molası-Yemek-Parti")
            {
                return RedirectToAction("NewCoffee", "Activities");
            }
            else
            {
                return RedirectToAction("SelectActivityCategory", "Activities");
            }
        }

        public dynamic SpeakerListOfEvent(List<User> AddedSpeaker)
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

            List<int> orders = new List<int>();
            int countAdded = AddedSpeaker.Count();
            int countAll = EventsSpeakers.Count();
            for (int k = 0; k < countAdded; k++)
            {
                for (int j = 0; j < countAll; j++)
                {
                    if (AddedSpeaker[k].Id == EventsSpeakers[j].Id)
                    {
                        orders.Add(j);
                    }
                }
            }

            var SortedOrder = orders.OrderByDescending(q => q).ToList();

            for (int p = 0; p < SortedOrder.Count; p++)
            {
                EventsSpeakers.RemoveAt(SortedOrder[p]);
            }

            List<User> SortedSpeakerList = new List<User>();
            SortedSpeakerList = EventsSpeakers.OrderBy(d => (d.Name + d.Surname)).ToList();

            return SortedSpeakerList;
        }

        public dynamic ResponsibleListOfEvent(List<User> AddedResponsibles)
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
            var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Acente").ToList();
            for (int i = 0; i < UserOfEvent.Count; i++)
            {
                if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                {
                    User newuser = db.User.Find(UserOfEvent[i].Id);
                    EventsSpeakers.Add(newuser);
                }
            }

            List<int> orders = new List<int>();
            int countAdded = AddedResponsibles.Count();
            int countAll = EventsSpeakers.Count();
            for (int k = 0; k < countAdded; k++)
            {
                for (int j = 0; j < countAll; j++)
                {
                    if (AddedResponsibles[k].Id == EventsSpeakers[j].Id)
                    {
                        orders.Add(j);
                    }
                }
            }

            var SortedOrder = orders.OrderByDescending(q => q).ToList();

            for (int p = 0; p < SortedOrder.Count; p++)
            {
                EventsSpeakers.RemoveAt(SortedOrder[p]);
            }

            List<User> SortedResponsibleList = new List<User>();
            SortedResponsibleList = EventsSpeakers.OrderBy(d => (d.Name + d.Surname)).ToList();

            return SortedResponsibleList;
        }

        public dynamic MapListOfEvent(List<Map> AddedPlaces)
        {
            // Login olan User Bilgleri
            User LoggedUser = (User)Session["LoggedIn"];

            // Tüm Event'ler'den logged in olunan Event'i bulduk
            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Tüm Eventlerin User listesinden logged in olunan Event'in User listesini çektik
            var AllMapList = db.MapEventActivity.ToList();
            var MapOfEvent = new List<Map>();
            for (int j = 0; j < AllMapList.Count; j++)
            {
                if (AllMapList[j].EventOrEventActivityID == IDofEvent[0].Id)
                {
                    Map newmap = db.Map.Find(AllMapList[j].MapID);
                    MapOfEvent.Add(newmap);
                }
            }

            List<int> orders = new List<int>();
            int countAdded = AddedPlaces.Count();
            int countAll = MapOfEvent.Count();
            for (int k = 0; k < countAdded; k++)
            {
                for (int j = 0; j < countAll; j++)
                {
                    if (AddedPlaces[k].Id == MapOfEvent[j].Id)
                    {
                        orders.Add(j);
                    }
                }
            }

            var SortedOrder = orders.OrderByDescending(q => q).ToList();

            for (int p = 0; p < SortedOrder.Count; p++)
            {
                MapOfEvent.RemoveAt(SortedOrder[p]);
            }

            List<Map> SortedMapList = new List<Map>();
            SortedMapList = MapOfEvent.OrderBy(d => (d.Name)).ToList();

            return SortedMapList;
        }

        public ActionResult CreateConference(Activity act, string edit, Guid? DeleteAfterEdit, string CompleteConference, Guid? Speakers, Guid? DeleteSpeaker, Guid? Responsibles, Guid? DeleteResponsible, Guid? Places, Guid? DeletePlace, string IsNew, List<User> AddedSpeakers, List<User> AddedResponsibles, List<Map> AddedPlaces)
        {
            Session["BackButton"] = Session["BackButton"];
            if (DeleteAfterEdit != null)
            {
                Session["DeletAfterEdit"] = DeleteAfterEdit;
            }
            else
            {
                Session["DeletAfterEdit"] = Session["DeletAfterEdit"];
            }

            if (edit == "edit" && IsNew != "newconference")
            {
                AddedSpeakers = (List<User>)TempData["spkrs"];
                Session["AddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = (List<User>)TempData["rspnbls"];
                Session["AddedResponsibles"] = AddedResponsibles;
                AddedPlaces = (List<Map>)TempData["plcs"];
                Session["AddedPlaces"] = AddedPlaces;
                act = (Activity)TempData["act"];
            }
            else if (IsNew == "newconference")
            {
                AddedSpeakers = new List<User>();
                Session["AddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = new List<User>();
                Session["AddedResponsibles"] = AddedResponsibles;
                AddedPlaces = new List<Map>();
                Session["AddedPlaces"] = AddedPlaces;
            }
            else
            {
                AddedSpeakers = (List<User>)Session["AddedSpeakers"];
                AddedResponsibles = (List<User>)Session["AddedResponsibles"];
                AddedPlaces = (List<Map>)Session["AddedPlaces"];
            }

            string CancelError = "Correct";
            if (DeleteSpeaker != null || DeleteResponsible != null || DeletePlace != null)
            {
                CancelError = "Error";
            }

            if (Speakers.ToString() != "00000000-0000-0000-0000-000000000000" && Speakers != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedSpeakers.Count; i++)
                {
                    if (AddedSpeakers[i].Id == Speakers)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedSpeakers.Count == 0)
                {
                    User newspeaker = db.User.Find(Speakers);
                    AddedSpeakers.Add(newspeaker);
                    Session["AddedSpeakers"] = AddedSpeakers;
                }
            }

            if (DeleteSpeaker.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteSpeaker != null)
            {
                User Sdel = db.User.Find(DeleteSpeaker);
                int count = AddedSpeakers.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedSpeakers[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "delete";
                    }
                }
                if (delete == "delete")
                {
                    AddedSpeakers.RemoveAt(order);
                    Session["AddedSpeakers"] = AddedSpeakers;
                }
            }

            if (Responsibles.ToString() != "00000000-0000-0000-0000-000000000000" && Responsibles != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedResponsibles.Count; i++)
                {
                    if (AddedResponsibles[i].Id == Responsibles)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedResponsibles.Count == 0)
                {
                    User newresponsible = db.User.Find(Responsibles);
                    AddedResponsibles.Add(newresponsible);
                    Session["AddedResponsibles"] = AddedResponsibles;
                }
            }

            if (DeleteResponsible.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteResponsible != null)
            {
                User Sdel = db.User.Find(DeleteResponsible);
                int count = AddedResponsibles.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedResponsibles[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedResponsibles.RemoveAt(order);
                    Session["AddedResponsibles"] = AddedResponsibles;
                }
            }

            if (Places.ToString() != "00000000-0000-0000-0000-000000000000" && Places != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedPlaces.Count; i++)
                {
                    if (AddedPlaces[i].Id == Places)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedPlaces.Count == 0)
                {
                    Map newplace = db.Map.Find(Places);
                    AddedPlaces.Add(newplace);
                    Session["AddedPlaces"] = AddedPlaces;
                }
            }

            if (DeletePlace.ToString() != "00000000-0000-0000-0000-000000000000" && DeletePlace != null)
            {
                Map Sdel = db.Map.Find(DeletePlace);
                int count = AddedPlaces.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedPlaces[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedPlaces.RemoveAt(order);
                    Session["AddedPlaces"] = AddedPlaces;
                }
            }

            ViewData["AddedSpeakers"] = AddedSpeakers;
            ViewData["AddedResponsibles"] = AddedResponsibles;
            ViewData["AddedPlaces"] = AddedPlaces;

            List<User> SpeakerList = SpeakerListOfEvent(AddedSpeakers);
            List<User> ResponsibleList = ResponsibleListOfEvent(AddedResponsibles);
            List<Map> MapList = MapListOfEvent(AddedPlaces);
            ViewData["SpeakerList"] = SpeakerList;
            ViewData["ResponsibleList"] = ResponsibleList;
            ViewData["MapList"] = MapList;

            if (ModelState.IsValid)
            {
                List<string> ValidationErrors = ConferenceValidation(act);
                if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompleteConference == "CompleteConference")
                {
                    //Burada Create Edicez
                    TempData["AddedSpeaker"] = AddedSpeakers;
                    TempData["AddedResponsible"] = AddedResponsibles;
                    TempData["AddedPlaces"] = AddedPlaces;
                    TempData["Model"] = act;
                    Session["DeletAfterEdit"] = Session["DeletAfterEdit"];
                    return RedirectToAction("ConferenceSubmit", "Activities");
                }
                else if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompleteConference != "CompleteConference" && edit != "edit")
                {
                    ViewData["ValidationErrors"] = ValidationErrors;
                    return View();
                }
                else if (edit == "edit")
                {
                    ViewData["ValidationErrors"] = ValidationErrors;
                    return View(act);
                }
                else
                {
                    ViewData["ValidationErrors"] = ValidationErrors;
                    return View();
                }
            }
            else
            {
                string correct = "Correct";
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ViewData["ValidationErrors"] = ValidationErrors;

                return View();
            }
        }

        public dynamic ConferenceValidation(Activity act)
        {
            string DateCompareError = "Correct";
            string SDateError = "Correct";
            string EDateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";
            string SummaryError = "Correct";


            if ((act.StartDate != null && act.EndDate != null))
            {
                string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                ViewData["result"] = result;
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                }
            }
            else if (act.StartDate == null && act.EndDate != null)
            {
                SDateError = "SDateError";
            }
            else if (act.StartDate != null && act.EndDate == null)
            {
                EDateError = "EDateError";
            }
            else
            {
                SDateError = "SDateError";
                EDateError = "EDateError";
            }

            if (act.Name == null)
            {
                NameError = "NameError";
            }

            if (act.Content == null)
            {
                ContentError = "ContentError";
            }

            if (act.Summary == null)
            {
                SummaryError = "SummaryError";
            }

            List<string> ValidationErrors = new List<string>();
            ValidationErrors.Add(DateCompareError);
            ValidationErrors.Add(SDateError);
            ValidationErrors.Add(EDateError);
            ValidationErrors.Add(NameError);
            ValidationErrors.Add(SummaryError);
            ValidationErrors.Add(ContentError);

            return ValidationErrors;
        }

        public ActionResult ConferenceSubmit()
        {
            Activity act = (Activity)TempData["Model"];
            List<User> AddedSpeakers = (List<User>)TempData["AddedSpeaker"];
            List<User> AddedResponsibles = (List<User>)TempData["AddedResponsible"];
            List<Map> AddedPlaces = (List<Map>)TempData["AddedPlaces"];

            if (ModelState.IsValid)
            {
                Guid deletePrevious = (Guid)Session["DeletAfterEdit"];
                if (deletePrevious.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    using (EventAppContext db = new EventAppContext())
                    {
                        // Activity Kaydediliyor
                        List<ActivityCategory> Category = db.ActivityCategory.ToList();
                        Guid CategoryID = Guid.Empty;
                        for (int i = 0; i < Category.Count; i++)
                        {
                            if (Category[i].Name == "Konferans")
                            {
                                CategoryID = Category[i].Id;
                            }
                        }

                        string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        act.StartDateTime = startdate;
                        act.EndDateTime = enddate;
                        act.ActivityCategoryID = CategoryID;
                        act.IsPostEventSurvey = false;
                        act.IsActive = true;

                        db.Activity.Add(act);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            //Activity ID'si oluştuktan sonra EventActivity'e kaydediliyor
                            User LoggedUser = (User)Session["LoggedIn"];
                            var fromAllEvents = db.Event.ToList();
                            var IDofEvent = fromAllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

                            EventActivity EActivity = new EventActivity();
                            EActivity.ActivityID = act.Id;
                            EActivity.EventID = IDofEvent[0].Id;
                            dbUE.EventActivity.Add(EActivity);
                            dbUE.SaveChanges();

                            using (EventAppContext dbAgenda = new EventAppContext())
                            {
                                // AgendaEventActivity kaydediliyor
                                AgendaEventActivity agenda = new AgendaEventActivity();
                                agenda.EventActivityID = EActivity.Id;
                                dbAgenda.AgendaEventActivity.Add(agenda);
                                dbAgenda.SaveChanges();
                            }

                            using (EventAppContext dbSP = new EventAppContext())
                            {
                                // AddedSpeakers kaydediliyor
                                for (int i = 0; i < AddedSpeakers.Count; i++)
                                {
                                    SpeakerEventActivity sea = new SpeakerEventActivity();
                                    sea.EventActivityID = EActivity.Id;
                                    sea.SpeakerID = AddedSpeakers[i].Id;
                                    dbSP.SpeakerEventActivity.Add(sea);
                                    dbSP.SaveChanges();
                                }
                            }

                            using (EventAppContext dbRS = new EventAppContext())
                            {
                                // AddedResponsibles kaydediliyor
                                for (int i = 0; i < AddedResponsibles.Count; i++)
                                {
                                    ResponsibleEventActivity rea = new ResponsibleEventActivity();
                                    rea.EventActivityID = EActivity.Id;
                                    rea.ResponsibleID = AddedResponsibles[i].Id;
                                    dbRS.ResponsibleEventActivity.Add(rea);
                                    dbRS.SaveChanges();
                                }
                            }

                            using (EventAppContext dbPL = new EventAppContext())
                            {
                                // AddedPlaces Kaydediliyor
                                for (int i = 0; i < AddedPlaces.Count; i++)
                                {
                                    MapEventActivity mea = new MapEventActivity();
                                    mea.EventOrEventActivityID = EActivity.Id;
                                    mea.MapID = AddedPlaces[i].Id;
                                    dbPL.MapEventActivity.Add(mea);
                                    dbPL.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    Activity ChangeThisActivity = db.Activity.Find(deletePrevious);
                    // Yeni Activity oluştur
                    List<ActivityCategory> Category = db.ActivityCategory.ToList();
                    Guid CategoryID = Guid.Empty;
                    for (int i = 0; i < Category.Count; i++)
                    {
                        if (Category[i].Name == "Konferans")
                        {
                            CategoryID = Category[i].Id;
                        }
                    }

                    string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                    DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                    string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                    DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                    act.StartDateTime = startdate;
                    act.EndDateTime = enddate;
                    act.ActivityCategoryID = CategoryID;
                    act.IsPostEventSurvey = false;
                    act.IsActive = true;

                    db.Activity.Add(act);
                    db.SaveChanges();

                    // Agenda-Speaker-Responsible-Map'leri sil
                    var AllEventActivities = db.EventActivity.ToList();
                    var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == deletePrevious).ToList();
                    Guid ThisEventActivityID = EditEventActivityID[0].Id;
                    EventActivity ThisEventActivity = db.EventActivity.Find(ThisEventActivityID);

                    var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                    var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < SpeakerList.Count; i++)
                    {
                        SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                        db.SpeakerEventActivity.Remove(deleteSpeaker);
                        db.SaveChanges();
                    }

                    var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                    var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < ResponsibleList.Count; i++)
                    {
                        ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                        db.ResponsibleEventActivity.Remove(deleteResponsible);
                        db.SaveChanges();
                    }

                    var AllAddedPlaceList = db.MapEventActivity.ToList();
                    var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < PlaceList.Count; i++)
                    {
                        MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                        db.MapEventActivity.Remove(deleteMap);
                        db.SaveChanges();
                    }

                    var AllAddedAgenda = db.AgendaEventActivity.ToList();
                    var AgendaList = AllAddedAgenda.Where(t => t.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < AgendaList.Count; i++)
                    {
                        AgendaEventActivity deleteagenda = db.AgendaEventActivity.Find(AgendaList[i].Id);
                        db.AgendaEventActivity.Remove(deleteagenda);
                        db.SaveChanges();
                    }

                    // EventActivity'deki Activity ID'yi sil
                    ThisEventActivity.ActivityID = act.Id;
                    db.SaveChanges();

                    // Eski Activity siliniyor
                    db.Activity.Remove(ChangeThisActivity);
                    db.SaveChanges();

                    // Agenda-Speaker-Responsible-Map'i oluştur
                    using (EventAppContext dbAgenda = new EventAppContext())
                    {
                        // AgendaEventActivity kaydediliyor
                        AgendaEventActivity agenda = new AgendaEventActivity();
                        agenda.EventActivityID = ThisEventActivity.Id;
                        dbAgenda.AgendaEventActivity.Add(agenda);
                        dbAgenda.SaveChanges();
                    }

                    using (EventAppContext dbSP = new EventAppContext())
                    {
                        // AddedSpeakers kaydediliyor
                        for (int i = 0; i < AddedSpeakers.Count; i++)
                        {
                            SpeakerEventActivity sea = new SpeakerEventActivity();
                            sea.EventActivityID = ThisEventActivity.Id;
                            sea.SpeakerID = AddedSpeakers[i].Id;
                            dbSP.SpeakerEventActivity.Add(sea);
                            dbSP.SaveChanges();
                        }
                    }

                    using (EventAppContext dbRS = new EventAppContext())
                    {
                        // AddedResponsibles kaydediliyor
                        for (int i = 0; i < AddedResponsibles.Count; i++)
                        {
                            ResponsibleEventActivity rea = new ResponsibleEventActivity();
                            rea.EventActivityID = ThisEventActivity.Id;
                            rea.ResponsibleID = AddedResponsibles[i].Id;
                            dbRS.ResponsibleEventActivity.Add(rea);
                            dbRS.SaveChanges();
                        }
                    }

                    using (EventAppContext dbPL = new EventAppContext())
                    {
                        // AddedPlaces Kaydediliyor
                        for (int i = 0; i < AddedPlaces.Count; i++)
                        {
                            MapEventActivity mea = new MapEventActivity();
                            mea.EventOrEventActivityID = ThisEventActivity.Id;
                            mea.MapID = AddedPlaces[i].Id;
                            dbPL.MapEventActivity.Add(mea);
                            dbPL.SaveChanges();
                        }
                    }
                }

                TempData["AddedSpeaker"] = AddedSpeakers;
                TempData["AddedResponsible"] = AddedResponsibles;
                TempData["AddedPlaces"] = AddedPlaces;
                TempData["Model"] = act;

                int countRespAct = RespAct();
                Session["AgendaCountS"] = countRespAct;

                return RedirectToAction("ConferenceDetailsEditable", "Activities");
            }

            int count = RespAct();
            Session["AgendaCountS"] = count;

            return View();
        }

        public ActionResult ConferenceDetailsEditable()
        {
            Activity act = (Activity)TempData["Model"];
            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["ActivityDetails"] = act;
            ViewData["AddedSpeaker"] = (List<User>)TempData["AddedSpeaker"];
            ViewData["AddedResponsible"] = (List<User>)TempData["AddedResponsible"];
            ViewData["AddedPlace"] = (List<Map>)TempData["AddedPlaces"];
            ViewData["Dates"] = StartEndTimes((Activity)TempData["Model"]);

            return View();
        }

        public ActionResult ConferenceDetailsOrg(Guid IDofActivity, string from, Guid? FromSpeaker)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["ActivityDetails"] = act;
            ViewData["AddedSpeaker"] = AddedSpeaker;
            ViewData["AddedResponsible"] = AddedResponsible;
            ViewData["AddedPlace"] = AddedPlaces;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["From"] = from;

            if (FromSpeaker != null)
            {
                ViewData["FromSpeaker"] = FromSpeaker;
            }
            else
            {
                ViewData["FromSpeaker"] = null;
            }
            return View();
        }

        public ActionResult ConferenceEdit(Guid? IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);


            TempData["spkrs"] = AddedSpeaker;
            TempData["rspnbls"] = AddedResponsible;
            TempData["plcs"] = AddedPlaces;
            TempData["act"] = act;
            Session["BackButton"] = "BackToList";
            string edit = "edit";

            return RedirectToAction("CreateConference", new { edit = edit, DeleteAfterEdit = IDofActivity });
        }

        public ActionResult DeleteConference(Guid ActId)
        {
            Guid deletePrevious = ActId;
            if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(deletePrevious);
                activity.IsActive = false;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult ConferenceDetails(Guid IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            bool scheduled = isScheduled(IDofActivity);

            ViewData["isScheduled"] = scheduled;
            ViewData["AddedSpeaker"] = AddedSpeaker;
            ViewData["AddedResponsible"] = AddedResponsible;
            ViewData["AddedPlace"] = AddedPlaces;
            ViewData["ActivityDetails"] = act;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["Documents"] = ActivitiesDocument(EditEventActivityID[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            return View();
        }

        public ActionResult RecoverConference(Guid IDofActivity)
        {
            Activity activity = db.Activity.Find(IDofActivity);
            activity.IsActive = true;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult DeleteConferencePermanently(Guid IDofActivity)
        {
            if (IDofActivity.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(IDofActivity);
                db.Activity.Remove(activity);
                db.SaveChanges();

                // EventActivity'den de siliniecek
                var AllEventActivities = db.EventActivity.ToList();
                var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
                Guid oldEventActivityID = EditEventActivityID[0].Id;
                EventActivity deleteEA = db.EventActivity.Find(oldEventActivityID);
                db.EventActivity.Remove(deleteEA);
                db.SaveChanges();

                var AllAgendaList = db.AgendaEventActivity.ToList();
                var deletefromagenda = AllAgendaList.Where(t => t.EventActivityID == oldEventActivityID).ToList();
                AgendaEventActivity isdeleting = db.AgendaEventActivity.Find(deletefromagenda[0].Id);
                db.AgendaEventActivity.Remove(isdeleting);
                db.SaveChanges();

                // Delete from SpeakerAEventActivity
                var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == oldEventActivityID).ToList();
                for (int i = 0; i < SpeakerList.Count; i++)
                {
                    SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                    db.SpeakerEventActivity.Remove(deleteSpeaker);
                    db.SaveChanges();
                }

                // Delete from ResponsibleEventActivity
                var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < ResponsibleList.Count; i++)
                {
                    ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                    db.ResponsibleEventActivity.Remove(deleteResponsible);
                    db.SaveChanges();
                }

                // Delete from MapEventActivity
                var AllAddedPlaceList = db.MapEventActivity.ToList();
                var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < PlaceList.Count; i++)
                {
                    MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                    db.MapEventActivity.Remove(deleteMap);
                    db.SaveChanges();
                }
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        // Presentation
        public ActionResult CreatePresentation(Activity act, string edit, Guid? DeleteAfterEdit, string CompletePresentation, Guid? Speakers, Guid? DeleteSpeaker, Guid? Responsibles, Guid? DeleteResponsible, Guid? Places, Guid? DeletePlace, string IsNew, List<User> AddedSpeakers, List<User> AddedResponsibles, List<Map> AddedPlaces)
        {
            Session["BackButton"] = Session["BackButton"];
            if (DeleteAfterEdit != null)
            {
                Session["PreDeletAfterEdit"] = DeleteAfterEdit;
            }
            else
            {
                Session["PreDeletAfterEdit"] = Session["PreDeletAfterEdit"];
            }

            if (edit == "edit" && IsNew != "newpresentation")
            {
                AddedSpeakers = (List<User>)TempData["spkrs"];
                Session["PreAddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = (List<User>)TempData["rspnbls"];
                Session["PreAddedResponsibles"] = AddedResponsibles;
                AddedPlaces = (List<Map>)TempData["plcs"];
                Session["PreAddedPlaces"] = AddedPlaces;
                act = (Activity)TempData["act"];
            }
            else if (IsNew == "newpresentation")
            {
                AddedSpeakers = new List<User>();
                Session["PreAddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = new List<User>();
                Session["PreAddedResponsibles"] = AddedResponsibles;
                AddedPlaces = new List<Map>();
                Session["PreAddedPlaces"] = AddedPlaces;
            }
            else
            {
                AddedSpeakers = (List<User>)Session["PreAddedSpeakers"];
                AddedResponsibles = (List<User>)Session["PreAddedResponsibles"];
                AddedPlaces = (List<Map>)Session["PreAddedPlaces"];
            }

            string CancelError = "Correct";
            if (DeleteSpeaker != null || DeleteResponsible != null || DeletePlace != null)
            {
                CancelError = "Error";
            }

            if (Speakers.ToString() != "00000000-0000-0000-0000-000000000000" && Speakers != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedSpeakers.Count; i++)
                {
                    if (AddedSpeakers[i].Id == Speakers)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedSpeakers.Count == 0)
                {
                    User newspeaker = db.User.Find(Speakers);
                    AddedSpeakers.Add(newspeaker);
                    Session["PreAddedSpeakers"] = AddedSpeakers;
                }
            }

            if (DeleteSpeaker.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteSpeaker != null)
            {
                User Sdel = db.User.Find(DeleteSpeaker);
                int count = AddedSpeakers.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedSpeakers[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "delete";
                    }
                }
                if (delete == "delete")
                {
                    AddedSpeakers.RemoveAt(order);
                    Session["PreAddedSpeakers"] = AddedSpeakers;
                }
            }

            if (Responsibles.ToString() != "00000000-0000-0000-0000-000000000000" && Responsibles != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedResponsibles.Count; i++)
                {
                    if (AddedResponsibles[i].Id == Responsibles)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedResponsibles.Count == 0)
                {
                    User newresponsible = db.User.Find(Responsibles);
                    AddedResponsibles.Add(newresponsible);
                    Session["PreAddedResponsibles"] = AddedResponsibles;
                }
            }

            if (DeleteResponsible.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteResponsible != null)
            {
                User Sdel = db.User.Find(DeleteResponsible);
                int count = AddedResponsibles.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedResponsibles[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedResponsibles.RemoveAt(order);
                    Session["PreAddedResponsibles"] = AddedResponsibles;
                }
            }

            if (Places.ToString() != "00000000-0000-0000-0000-000000000000" && Places != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedPlaces.Count; i++)
                {
                    if (AddedPlaces[i].Id == Places)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedPlaces.Count == 0)
                {
                    Map newplace = db.Map.Find(Places);
                    AddedPlaces.Add(newplace);
                    Session["PreAddedPlaces"] = AddedPlaces;
                }
            }

            if (DeletePlace.ToString() != "00000000-0000-0000-0000-000000000000" && DeletePlace != null)
            {
                Map Sdel = db.Map.Find(DeletePlace);
                int count = AddedPlaces.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedPlaces[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedPlaces.RemoveAt(order);
                    Session["PreAddedPlaces"] = AddedPlaces;
                }
            }

            ViewData["PreAddedSpeakers"] = AddedSpeakers;
            ViewData["PreAddedResponsibles"] = AddedResponsibles;
            ViewData["PreAddedPlaces"] = AddedPlaces;

            List<User> SpeakerList = SpeakerListOfEvent(AddedSpeakers);
            List<User> ResponsibleList = ResponsibleListOfEvent(AddedResponsibles);
            List<Map> MapList = MapListOfEvent(AddedPlaces);
            ViewData["PreSpeakerList"] = SpeakerList;
            ViewData["PreResponsibleList"] = ResponsibleList;
            ViewData["PreMapList"] = MapList;

            if (ModelState.IsValid)
            {
                List<string> ValidationErrors = PresentationValidation(act);
                if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompletePresentation == "CompletePresentation")
                {
                    //Burada Create Edicez
                    TempData["AddedSpeaker"] = AddedSpeakers;
                    TempData["AddedResponsible"] = AddedResponsibles;
                    TempData["AddedPlaces"] = AddedPlaces;
                    TempData["Model"] = act;
                    Session["PreDeletAfterEdit"] = Session["PreDeletAfterEdit"];
                    return RedirectToAction("PresentationSubmit", "Activities");
                }
                else if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompletePresentation != "CompletePresentation" && edit != "edit")
                {
                    ViewData["PreValidationErrors"] = ValidationErrors;
                    return View();
                }
                else if (edit == "edit")
                {
                    ViewData["PreValidationErrors"] = ValidationErrors;
                    return View(act);
                }
                else
                {
                    ViewData["PreValidationErrors"] = ValidationErrors;
                    return View();
                }
            }
            else
            {
                string correct = "Correct";
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ViewData["PreValidationErrors"] = ValidationErrors;
                return View();
            }
        }

        public dynamic PresentationValidation(Activity act)
        {
            string DateCompareError = "Correct";
            string SDateError = "Correct";
            string EDateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";
            string SummaryError = "Correct";


            if ((act.StartDate != null && act.EndDate != null))
            {
                string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                ViewData["Preresult"] = result;
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                }
            }
            else if (act.StartDate == null && act.EndDate != null)
            {
                SDateError = "SDateError";
            }
            else if (act.StartDate != null && act.EndDate == null)
            {
                EDateError = "EDateError";
            }
            else
            {
                SDateError = "SDateError";
                EDateError = "EDateError";
            }

            if (act.Name == null)
            {
                NameError = "NameError";
            }

            if (act.Content == null)
            {
                ContentError = "ContentError";
            }

            if (act.Summary == null)
            {
                SummaryError = "SummaryError";
            }

            List<string> ValidationErrors = new List<string>();
            ValidationErrors.Add(DateCompareError);
            ValidationErrors.Add(SDateError);
            ValidationErrors.Add(EDateError);
            ValidationErrors.Add(NameError);
            ValidationErrors.Add(SummaryError);
            ValidationErrors.Add(ContentError);

            return ValidationErrors;
        }

        public ActionResult PresentationSubmit()
        {
            Activity act = (Activity)TempData["Model"];
            List<User> AddedSpeakers = (List<User>)TempData["AddedSpeaker"];
            List<User> AddedResponsibles = (List<User>)TempData["AddedResponsible"];
            List<Map> AddedPlaces = (List<Map>)TempData["AddedPlaces"];

            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    Guid deletePrevious = (Guid)Session["PreDeletAfterEdit"];
                    if (deletePrevious.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        // Activity Kaydediliyor
                        List<ActivityCategory> Category = db.ActivityCategory.ToList();
                        Guid CategoryID = Guid.Empty;
                        for (int i = 0; i < Category.Count; i++)
                        {
                            if (Category[i].Name == "Sunum")
                            {
                                CategoryID = Category[i].Id;
                            }
                        }

                        string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        act.StartDateTime = startdate;
                        act.EndDateTime = enddate;
                        act.ActivityCategoryID = CategoryID;
                        act.IsPostEventSurvey = false;
                        act.IsActive = true;

                        db.Activity.Add(act);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            //Activity ID'si oluştuktan sonra EventActivity'e kaydediliyor
                            User LoggedUser = (User)Session["LoggedIn"];
                            var fromAllEvents = dbUE.Event.ToList();
                            var IDofEvent = fromAllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

                            EventActivity EActivity = new EventActivity();
                            EActivity.ActivityID = act.Id;
                            EActivity.EventID = IDofEvent[0].Id;
                            dbUE.EventActivity.Add(EActivity);
                            dbUE.SaveChanges();

                            using (EventAppContext dbAgenda = new EventAppContext())
                            {
                                // AgendaEventActivity kaydediliyor
                                AgendaEventActivity agenda = new AgendaEventActivity();
                                agenda.EventActivityID = EActivity.Id;
                                dbAgenda.AgendaEventActivity.Add(agenda);
                                dbAgenda.SaveChanges();
                            }

                            using (EventAppContext dbSP = new EventAppContext())
                            {
                                // AddedSpeakers kaydediliyor
                                for (int i = 0; i < AddedSpeakers.Count; i++)
                                {
                                    SpeakerEventActivity sea = new SpeakerEventActivity();
                                    sea.EventActivityID = EActivity.Id;
                                    sea.SpeakerID = AddedSpeakers[i].Id;
                                    dbSP.SpeakerEventActivity.Add(sea);
                                    dbSP.SaveChanges();
                                }
                            }

                            using (EventAppContext dbRS = new EventAppContext())
                            {
                                // AddedResponsibles kaydediliyor
                                for (int i = 0; i < AddedResponsibles.Count; i++)
                                {
                                    ResponsibleEventActivity rea = new ResponsibleEventActivity();
                                    rea.EventActivityID = EActivity.Id;
                                    rea.ResponsibleID = AddedResponsibles[i].Id;
                                    dbRS.ResponsibleEventActivity.Add(rea);
                                    dbRS.SaveChanges();
                                }
                            }

                            using (EventAppContext dbPL = new EventAppContext())
                            {
                                // AddedPlaces Kaydediliyor
                                for (int i = 0; i < AddedPlaces.Count; i++)
                                {
                                    MapEventActivity mea = new MapEventActivity();
                                    mea.EventOrEventActivityID = EActivity.Id;
                                    mea.MapID = AddedPlaces[i].Id;
                                    dbPL.MapEventActivity.Add(mea);
                                    dbPL.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        Activity ChangeThisActivity = db.Activity.Find(deletePrevious);
                        // Yeni Activity oluştur
                        List<ActivityCategory> Category = db.ActivityCategory.ToList();
                        Guid CategoryID = Guid.Empty;
                        for (int i = 0; i < Category.Count; i++)
                        {
                            if (Category[i].Name == "Sunum")
                            {
                                CategoryID = Category[i].Id;
                            }
                        }

                        string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        act.StartDateTime = startdate;
                        act.EndDateTime = enddate;
                        act.ActivityCategoryID = CategoryID;
                        act.IsPostEventSurvey = false;
                        act.IsActive = true;

                        db.Activity.Add(act);
                        db.SaveChanges();

                        // Agenda-Speaker-Responsible-Map'leri sil
                        var AllEventActivities = db.EventActivity.ToList();
                        var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == deletePrevious).ToList();
                        Guid ThisEventActivityID = EditEventActivityID[0].Id;
                        EventActivity ThisEventActivity = db.EventActivity.Find(ThisEventActivityID);

                        var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                        var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                        for (int i = 0; i < SpeakerList.Count; i++)
                        {
                            SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                            db.SpeakerEventActivity.Remove(deleteSpeaker);
                            db.SaveChanges();
                        }

                        var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                        var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                        for (int i = 0; i < ResponsibleList.Count; i++)
                        {
                            ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                            db.ResponsibleEventActivity.Remove(deleteResponsible);
                            db.SaveChanges();
                        }

                        var AllAddedPlaceList = db.MapEventActivity.ToList();
                        var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == ThisEventActivityID).ToList();
                        for (int i = 0; i < PlaceList.Count; i++)
                        {
                            MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                            db.MapEventActivity.Remove(deleteMap);
                            db.SaveChanges();
                        }

                        var AllAddedAgenda = db.AgendaEventActivity.ToList();
                        var AgendaList = AllAddedAgenda.Where(t => t.EventActivityID == ThisEventActivityID).ToList();
                        for (int i = 0; i < AgendaList.Count; i++)
                        {
                            AgendaEventActivity deleteagenda = db.AgendaEventActivity.Find(AgendaList[i].Id);
                            db.AgendaEventActivity.Remove(deleteagenda);
                            db.SaveChanges();
                        }

                        // EventActivity'deki Activity ID'yi sil
                        ThisEventActivity.ActivityID = act.Id;
                        db.SaveChanges();

                        // Eski Activity siliniyor
                        db.Activity.Remove(ChangeThisActivity);
                        db.SaveChanges();

                        // Agenda-Speaker-Responsible-Map'i oluştur
                        using (EventAppContext dbAgenda = new EventAppContext())
                        {
                            // AgendaEventActivity kaydediliyor
                            AgendaEventActivity agenda = new AgendaEventActivity();
                            agenda.EventActivityID = ThisEventActivity.Id;
                            dbAgenda.AgendaEventActivity.Add(agenda);
                            dbAgenda.SaveChanges();
                        }

                        using (EventAppContext dbSP = new EventAppContext())
                        {
                            // AddedSpeakers kaydediliyor
                            for (int i = 0; i < AddedSpeakers.Count; i++)
                            {
                                SpeakerEventActivity sea = new SpeakerEventActivity();
                                sea.EventActivityID = ThisEventActivity.Id;
                                sea.SpeakerID = AddedSpeakers[i].Id;
                                dbSP.SpeakerEventActivity.Add(sea);
                                dbSP.SaveChanges();
                            }
                        }

                        using (EventAppContext dbRS = new EventAppContext())
                        {
                            // AddedResponsibles kaydediliyor
                            for (int i = 0; i < AddedResponsibles.Count; i++)
                            {
                                ResponsibleEventActivity rea = new ResponsibleEventActivity();
                                rea.EventActivityID = ThisEventActivity.Id;
                                rea.ResponsibleID = AddedResponsibles[i].Id;
                                dbRS.ResponsibleEventActivity.Add(rea);
                                dbRS.SaveChanges();
                            }
                        }

                        using (EventAppContext dbPL = new EventAppContext())
                        {
                            // AddedPlaces Kaydediliyor
                            for (int i = 0; i < AddedPlaces.Count; i++)
                            {
                                MapEventActivity mea = new MapEventActivity();
                                mea.EventOrEventActivityID = ThisEventActivity.Id;
                                mea.MapID = AddedPlaces[i].Id;
                                dbPL.MapEventActivity.Add(mea);
                                dbPL.SaveChanges();
                            }
                        }
                    }
                }

                TempData["AddedSpeaker"] = AddedSpeakers;
                TempData["AddedResponsible"] = AddedResponsibles;
                TempData["AddedPlaces"] = AddedPlaces;
                TempData["Model"] = act;

                int countRespAct = RespAct();
                Session["AgendaCountS"] = countRespAct;

                return RedirectToAction("PresentationDetailsEditable", "Activities");
            }

            int count = RespAct();
            Session["AgendaCountS"] = count;

            return View();
        }

        public ActionResult PresentationDetailsEditable()
        {
            Activity act = (Activity)TempData["Model"];
            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["PreActivityDetails"] = act;
            ViewData["PreAddedSpeaker"] = (List<User>)TempData["AddedSpeaker"];
            ViewData["PreAddedResponsible"] = (List<User>)TempData["AddedResponsible"];
            ViewData["PreAddedPlace"] = (List<Map>)TempData["AddedPlaces"];
            ViewData["Dates"] = StartEndTimes((Activity)TempData["Model"]);
            return View();
        }

        public ActionResult PresentationDetailsOrg(Guid IDofActivity, string from, Guid? FromSpeaker)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["PreActivityDetails"] = act;
            ViewData["PreAddedSpeaker"] = AddedSpeaker;
            ViewData["PreAddedResponsible"] = AddedResponsible;
            ViewData["PreAddedPlace"] = AddedPlaces;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["From"] = from;

            if (FromSpeaker != null)
            {
                ViewData["FromSpeaker"] = FromSpeaker;
            }
            else
            {
                ViewData["FromSpeaker"] = null;
            }

            return View();
        }

        public ActionResult PresentationEdit(Guid? IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);


            TempData["spkrs"] = AddedSpeaker;
            TempData["rspnbls"] = AddedResponsible;
            TempData["plcs"] = AddedPlaces;
            TempData["act"] = act;
            Session["BackButton"] = "BackToList";
            string edit = "edit";

            return RedirectToAction("CreatePresentation", new { edit = edit, DeleteAfterEdit = IDofActivity });
        }

        public ActionResult DeletePresentation(Guid ActId)
        {
            Guid deletePrevious = ActId;
            if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(deletePrevious);
                activity.IsActive = false;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult PresentationDetails(Guid IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            bool scheduled = isScheduled(IDofActivity);

            ViewData["isScheduled"] = scheduled;
            ViewData["PreAddedSpeaker"] = AddedSpeaker;
            ViewData["PreAddedResponsible"] = AddedResponsible;
            ViewData["PreAddedPlace"] = AddedPlaces;
            ViewData["PreActivityDetails"] = act;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["Documents"] = ActivitiesDocument(EditEventActivityID[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];
            return View();
        }

        public ActionResult RecoverPresentation(Guid IDofActivity)
        {
            Activity activity = db.Activity.Find(IDofActivity);
            activity.IsActive = true;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult DeletePresentationPermanently(Guid IDofActivity)
        {
            if (IDofActivity.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(IDofActivity);
                db.Activity.Remove(activity);
                db.SaveChanges();

                // EventActivity'den de siliniecek
                var AllEventActivities = db.EventActivity.ToList();
                var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
                Guid oldEventActivityID = EditEventActivityID[0].Id;
                EventActivity deleteEA = db.EventActivity.Find(oldEventActivityID);
                db.EventActivity.Remove(deleteEA);
                db.SaveChanges();

                var AllAgendaList = db.AgendaEventActivity.ToList();
                var deletefromagenda = AllAgendaList.Where(t => t.EventActivityID == oldEventActivityID).ToList();
                AgendaEventActivity isdeleting = db.AgendaEventActivity.Find(deletefromagenda[0].Id);
                db.AgendaEventActivity.Remove(isdeleting);
                db.SaveChanges();

                // Delete from SpeakerAEventActivity
                var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == oldEventActivityID).ToList();
                for (int i = 0; i < SpeakerList.Count; i++)
                {
                    SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                    db.SpeakerEventActivity.Remove(deleteSpeaker);
                    db.SaveChanges();
                }

                // Delete from ResponsibleEventActivity
                var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < ResponsibleList.Count; i++)
                {
                    ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                    db.ResponsibleEventActivity.Remove(deleteResponsible);
                    db.SaveChanges();
                }

                // Delete from MapEventActivity
                var AllAddedPlaceList = db.MapEventActivity.ToList();
                var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < PlaceList.Count; i++)
                {
                    MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                    db.MapEventActivity.Remove(deleteMap);
                    db.SaveChanges();
                }
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        // ActivitityPage'de Scheduled olup olmadığı aşağıda sorgulanıyor
        public dynamic isScheduled(Guid? IDofActivity)
        {
            // EventActivityID
            var AllEventActivities = db.EventActivity.ToList();
            var ThisEventActivity = AllEventActivities.Where(q => q.ActivityID == IDofActivity).ToList();

            // UserEventActivityID
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // IsScheduled?
            var AllUserEventActivity = db.UserEventActivity.ToList();
            var scheduledornot = AllUserEventActivity.Where(k => k.EventActivityID == ThisEventActivity[0].Id && k.UserEventID == thisUserEvent[0].Id).ToList();

            bool isScheduled;
            if (scheduledornot.Count == 1)
            {
                isScheduled = true;
            }
            else
            {
                isScheduled = false;
            }

            return isScheduled;
        }

        public dynamic ActivitiesDocument(Guid IDofEventActivity)
        {
            var AllDocumentEventActivity = db.DocumentEventActivity.ToList();
            var isexist = AllDocumentEventActivity.Where(k => k.EventOrEventActivityID == IDofEventActivity).ToList();
            List<Document> ActDoc = new List<Document>();
            if (isexist.Count != 0)
            {
                for (int i = 0; i < isexist.Count; i++)
                {
                    Document addthis = db.Document.Find(isexist[i].DocumentID);
                    ActDoc.Add(addthis);
                }
            }

            List<Document> Sorted = new List<Document>();
            Sorted = ActDoc.OrderBy(d => (d.Name)).ToList();

            var FormatList = db.DocumentFormat.ToList();
            List<string> TypeOfDoc = new List<string>();
            for (int j = 0; j < Sorted.Count; j++)
            {
                var CheckDocType = FormatList.Where(k => k.Id == ActDoc[j].DocumentFormatID).ToList();
                DocumentFormat add = db.DocumentFormat.Find(CheckDocType[0].Id);
                TypeOfDoc.Add(add.Name);
            }

            TempData["TypeOfDoc"] = TypeOfDoc;

            return Sorted;
        }

        public dynamic RespAct()
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

            var AllRespoEA = db.ResponsibleEventActivity.ToList();
            int countActivities = 0;
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                var isResp = AllRespoEA.Where(k => k.EventActivityID == ThisEventActivities[i].Id && k.ResponsibleID == LoggedUser.Id).ToList();
                if (isResp.Count == 1)
                {
                    countActivities = countActivities + 1;
                }
            }

            return (countActivities);
        }

        public dynamic RoutingFrom(string from, Guid? FromSpeaker)
        {
            if (from == "RespActList")
            {
                return RedirectToAction("RespActList", "AgendaEventActivities");
            }
            else if (from == "ActivityListEditable")
            {
                return RedirectToAction("ActivityListEditable", "Activities");
            }
            else if (FromSpeaker != null)
            {
                return RedirectToAction("SpeakerDetailsOrg", "Users", new { sprdet = FromSpeaker });
            }

            return 111;
        }

        // Anket
        public dynamic ActivityList()
        {
            // Buraya anketin uygulanabileceği tüm Activity Category'leri eklenmeli AYNISI DOCUMENT İÇİN DE YAPILACAK
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(k => k.Name == "Sunum").ToList();


            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEventInfo = db.Event.Find(TE[0].Id);

            var AllEventActivities = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivities.Count; i++)
            {
                if (AllEventActivities[i].EventID == thisEvent[0].Id)
                {
                    EventActivity addthis = db.EventActivity.Find(AllEventActivities[i].Id);
                    ThisEventActivities.Add(addthis);
                }
            }

            var AllActivities = db.Activity.ToList();
            List<Activity> ThisEventsActivities = new List<Activity>();
            for (int k = 0; k < ThisEventActivities.Count; k++)
            {
                Activity add = db.Activity.Find(ThisEventActivities[k].ActivityID);
                // Buraya da sunum ve konferans dışındaki ilgili activitiy'ler eklenecek
                if ((add.ActivityCategoryID == PresentationType[0].Id || add.ActivityCategoryID == ConferenceType[0].Id) && add.IsActive == true)
                {
                    ThisEventsActivities.Add(add);
                }
            }

            List<Activity> SortedActivityList = new List<Activity>();
            SortedActivityList = ThisEventsActivities.OrderBy(d => (d.Name)).ToList();
            TempData["EventInfo"] = ThisEventInfo;
            return SortedActivityList;
        }

        public ActionResult CreateSurvey()
        {
            List<Activity> RegisteredActivities = ActivityList();
            ViewData["EventInfo"] = (Event)TempData["EventInfo"];
            ViewData["RegisteredActivities"] = RegisteredActivities;
            ViewData["NameError"] = "Correct";
            ViewData["ContentError"] = "Correct";
            ViewData["DateError"] = "Correct";
            return View();
        }

        [HttpPost]
        public ActionResult CreateSurvey(Activity srvy, Guid SelectedActivity)
        {
            List<Activity> RegisteredActivities = ActivityList();
            Event EventInfo = (Event)TempData["EventInfo"];
            ViewData["EventInfo"] = EventInfo;
            ViewData["RegisteredActivities"] = RegisteredActivities;

            int datecheck = 0;
            string DateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";

            if (srvy.EndDate != null)
            {
                String currenttime = DateTime.Now.ToLongTimeString();
                String currentday = DateTime.Now.ToLongDateString();

                string[] timedivide = currenttime.Split(new[] { " " }, StringSplitOptions.None);

                string currenthour = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }

                string currentdatetime = currentday + " " + currenthour;
                DateTime CurrentTime = Convert.ToDateTime(currentdatetime);

                string day = srvy.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                string[] dayandtime = day.Split(new[] { " - " }, StringSplitOptions.None);
                string[] daydivide = dayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[0];

                string hournormal = dayandtime[1];
                srvy.EndDate = daynormal + " " + hournormal;
                DateTime enddate = Convert.ToDateTime(srvy.EndDate);

                datecheck = DateTime.Compare(CurrentTime, enddate);

                if (datecheck >= 0)
                {
                    DateError = "DateError";
                }
            }

            if (srvy.Name == null || srvy.Content == null || DateError == "DateError")
            {
                if (srvy.Name == null)
                {
                    NameError = "NameError";
                }
                if (srvy.Content == null)
                {
                    ContentError = "ContentError";
                }
                ViewData["NameError"] = NameError;
                ViewData["ContentError"] = ContentError;
                ViewData["DateError"] = DateError;
                return View(srvy);
            }
            else
            {
                if (srvy.EndDate != null)
                {
                    DateTime enddate = Convert.ToDateTime(srvy.EndDate);
                    srvy.EndDateTime = enddate;

                    string changedate = srvy.EndDate;
                    string[] divide = changedate.Split(new[] { "-" }, StringSplitOptions.None);
                    string[] divide2 = divide[2].Split(new[] { " " }, StringSplitOptions.None);
                    string month1 = "*" + divide[1] + "*";
                    string month = month1.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
                    srvy.EndDate = divide2[0] + " " + month + " " + divide[0] + " - " + divide2[1];
                }

                var TypeOfActivity = db.ActivityCategory.ToList();
                var SurveyType = TypeOfActivity.Where(k => k.Name == "Anket").ToList();
                srvy.ActivityCategoryID = SurveyType[0].Id;

                srvy.EventOrEventActivityForSurveyAndDiscussion = SelectedActivity;
                srvy.IsActive = true;
                srvy.IsPostEventSurvey = false;
                db.Activity.Add(srvy);
                db.SaveChanges();

                EventActivity EA = new EventActivity();
                EA.EventID = EventInfo.Id;
                EA.ActivityID = srvy.Id;
                EA.IsActive = false;
                db.EventActivity.Add(EA);
                db.SaveChanges();

                return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = srvy.Id });
            }
        }

        public ActionResult CreateSurveyDetails(Guid thissurvey, string PageDisplay)
        {
            Session["FromPageSurvey"] = (string)Session["FromPageSurvey"];

            Activity ThisSurvey = db.Activity.Find(thissurvey);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyEventID"] = thisEA[0].Id;

            List<SurveyQuestionType> QuestionTypes = db.SurveyQuestionType.ToList();
            ViewData["QuesitonTypes"] = QuestionTypes;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];


            ViewData["ChoiceError"] = "Correct";
            
            if (PageDisplay == null)
            {
                ViewData["PageDisplay"] = "Default";
            }
            else if (PageDisplay == "Classic")
            {
                ViewData["PageDisplay"] = "Classic";
                ViewData["QuestionError"] = "QuestionError";
            }
            else if (PageDisplay == "MC1Select")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MC1Select";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoices"] = TempData["SavedChoices"];
                ViewData["OrderSavedChoices"] = TempData["OrderSavedChoices"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["Model"];
                return View(Model);
            }
            else if (PageDisplay == "MCMSelect")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MCMSelect";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoicesMCM"] = TempData["SavedChoicesMCM"];
                ViewData["OrderSavedChoicesMCM"] = TempData["OrderSavedChoicesMCM"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["ModelMCM"];
                return View(Model);
            }
            else if(PageDisplay == "DelMC1Select")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MC1Select";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoices"] = TempData["SavedChoices"];
                ViewData["OrderSavedChoices"] = TempData["OrderSavedChoices"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["Model"];
                return View(Model);
            }
            else if (PageDisplay == "DelMCMSelect")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MCMSelect";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoicesMCM"] = TempData["SavedChoicesMCM"];
                ViewData["OrderSavedChoicesMCM"] = TempData["OrderSavedChoicesMCM"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["ModelMCM"];
                return View(Model);
            }
            
            return View();
        }

        public dynamic ControllerToSurvey(Guid SurveyID)
        {
            // Kayıt gerçekleştirilen Survey Soruları çıkarılıyor
            List<SurveyActivityQuestion> AllQuestions = db.SurveyActivityQuestion.ToList();
            List<SurveyActivityQuestion> ThisSurveyQuesitions = new List<SurveyActivityQuestion>();
            for (int i = 0; i < AllQuestions.Count; i++)
            {
                if (AllQuestions[i].ActivityID == SurveyID)
                {
                    ThisSurveyQuesitions.Add(AllQuestions[i]);
                }                
            }

            List<SurveyActivityQuestion> SortedQuestions = new List<SurveyActivityQuestion>();
            SortedQuestions = ThisSurveyQuesitions.OrderByDescending(q => q.Order).ToList();
            TempData["SortedSurveyQuestions"] = SortedQuestions;

            var QTYPE = db.SurveyQuestionType.ToList();

            // Sırasıyla Question Type'lar çıkarılıyor
            List<string> TypeOfQuestionByOrder = new List<string>();
            for (int q = 0; q < SortedQuestions.Count; q++)
            {
                var ThisType = QTYPE.Where(w => w.Id == SortedQuestions[q].SurveyQuestionTypeID).ToList();
                TypeOfQuestionByOrder.Add(ThisType[0].Name);
            }
            TempData["TypeOfQuestionByOrder"] = TypeOfQuestionByOrder;

            var options = db.SurveyQuestionOption.ToList();
            List<int> OptionNumberOfQuestions = new List<int>();
            List<SurveyQuestionOption> QuestionOptions = new List<SurveyQuestionOption>();
            for (int j = 0; j < SortedQuestions.Count; j++)
            {
                var Number = options.Where(k => k.SurveyActivityQuestionID == SortedQuestions[j].Id).ToList();
                OptionNumberOfQuestions.Add(Number.Count);
                if (Number.Count > 0)
                {
                    for (int t = 0; t < Number.Count; t++)
                    {
                        SurveyQuestionOption AddOption = db.SurveyQuestionOption.Find(Number[t].Id);
                        QuestionOptions.Add(AddOption);
                    }
                }
                Number = null;
            }

            List<SurveyQuestionOption> SortedOptions = new List<SurveyQuestionOption>();
            SortedOptions = QuestionOptions.OrderByDescending(q => q.Order).ToList();

            TempData["OptionNumberOfQuestions"] = OptionNumberOfQuestions;
            TempData["QuestionOptions"] = SortedOptions;

            return 111;
        }

        public ActionResult SaveClassic(Guid SurveyID, SurveyActivityQuestion QQQ)
        {
            if (QQQ.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
                return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "Classic"});
            }
            var QuestionCategories = db.SurveyQuestionType.ToList();
            var ThisCategory = QuestionCategories.Where(t => t.Name == "Klasik").ToList();

            int Sequence = SurveyQuestionNumber(SurveyID);

            QQQ.SurveyQuestionTypeID = ThisCategory[0].Id;
            QQQ.ActivityID = SurveyID;
            QQQ.Order = Sequence + 1;

            db.SurveyActivityQuestion.Add(QQQ);
            db.SaveChanges();

            return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID});
        }

        public ActionResult CancelClassic()
        {
            Guid CancelID = (Guid)TempData["CancelClassic"];
            return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = CancelID });
        }

        public dynamic SurveyQuestionNumber(Guid SurveyID)
        {
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var ThisQuestions = AllQuestions.Where(k => k.ActivityID == SurveyID);
            int SavedQuestionNumber = ThisQuestions.Count();
            return SavedQuestionNumber;
        }

        public ActionResult SaveMC1Select(Guid? SurveyID, Guid? ContSurveyID, Guid? DelSurveyId, Guid? CancelSurveyID, string Choice, SurveyActivityQuestion MC)
        {
            if (CancelSurveyID != null)
            {
                Session["SavedChoices"] = null;
                Session["OrderSavedChoices"] = null;
                return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = CancelSurveyID });
            }
            Session["ModelMC1"] = MC;
            if (ContSurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoices"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];
                
                // Question girilmiş mi diye kontrol ediyor
                if (MC.Quesiton == null)
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }

                    TempData["QuestionError"] = "QuestionError";
                    TempData["Model"] = MC;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MC1Select" });
                }
                else
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }

                    TempData["Model"] = MC;
                    TempData["QuestionError"] = "Correct";
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MC1Select" });
                }   
            }
            else if (SurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoices"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];
                
                if (SavedChoices == null && Choice == "" && MC.Quesiton == null)
                {
                    // boş-boş
                    TempData["ChoiceError"] = "No Choices";
                }
                else if (SavedChoices == null && Choice == "" && MC.Quesiton != null)
                {
                    // boş-boş
                    TempData["ChoiceError"] = "No Choices";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    TempData["Model"] = MC;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton == null)
                {
                    // boş-dolu
                    TempData["ChoiceError"] = "Only Choice";
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton != null)
                {
                    // boş-dolu
                    TempData["Model"] = MC;
                    SavedChoices = new List<string>();
                    OrderSavedChoices = new List<int>();
                    SavedChoices.Add(Choice);
                    int sequence = OrderSavedChoices.Count + 1;
                    OrderSavedChoices.Add(sequence);
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton != null)
                {
                    // dolu-boş
                    TempData["Model"] = MC;
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }

                if (MC.Quesiton == null)
                {
                    TempData["QuestionError"] = "QuestionError";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else
                {
                    Guid ThisSurvey = (Guid)TempData["SurveyID"];
                    var QuestionCategories = db.SurveyQuestionType.ToList();
                    var ThisCategory = QuestionCategories.Where(t => t.Name == "Çoktan seçmeli tek seçim").ToList();
                    int Sequence = SurveyQuestionNumber(ThisSurvey);

                    MC.SurveyQuestionTypeID = ThisCategory[0].Id;
                    MC.ActivityID = ThisSurvey;
                    MC.Order = Sequence + 1;

                    db.SurveyActivityQuestion.Add(MC);
                    db.SaveChanges();

                    if (Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int seq = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(seq);
                    }

                    for (int i = 0; i < SavedChoices.Count; i++)
                    {
                        SurveyQuestionOption AddThisOption = new SurveyQuestionOption();
                        AddThisOption.SurveyActivityQuestionID = MC.Id;
                        AddThisOption.Option = SavedChoices[i];
                        AddThisOption.Order = OrderSavedChoices[i];
                        db.SurveyQuestionOption.Add(AddThisOption);
                        db.SaveChanges();
                    }
                    Session["SavedChoices"] = null;
                    Session["OrderSavedChoices"] = null;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID });
                }
            }
            return View();
        }

        public ActionResult DelChoiceMC1(string DeleteChoice, Guid? DelSurveyId)
        {
            SurveyActivityQuestion MC = (SurveyActivityQuestion)Session["ModelMC1"];

            int DelThis = Convert.ToInt32(DeleteChoice);

            List<string> SavedChoices = (List<string>)Session["SavedChoices"];
            List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];
            OrderSavedChoices.Remove(DelThis);
            string del = SavedChoices[DelThis-1];
            SavedChoices.Remove(del);

            for (int i = 0; i < OrderSavedChoices.Count; i++)
            {
                if (OrderSavedChoices[i] > DelThis)
                {
                    OrderSavedChoices[i] = OrderSavedChoices[i] - 1;
                }
            }


            TempData["SavedChoices"] = SavedChoices;
            TempData["OrderSavedChoices"] = OrderSavedChoices;
            Session["SavedChoices"] = SavedChoices;
            Session["OrderSavedChoices"] = OrderSavedChoices;
            if (MC.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
            }
            else
            {
                TempData["QuestionError"] = "Correct";
            }
            TempData["Model"] = MC;

            return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = DelSurveyId, PageDisplay = "DelMC1Select" });

        }
        
        public ActionResult SaveMCMSelect(Guid? SurveyID, Guid? ContSurveyID, Guid? DelSurveyId, Guid? CancelSurveyID, string Choice, SurveyActivityQuestion MC)
        {
            if (CancelSurveyID != null)
            {
                Session["SavedChoicesMCM"] = null;
                Session["OrderSavedChoicesMCM"] = null;
                return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = CancelSurveyID });
            }
            Session["ModelMCMSelect"] = MC;
            if (ContSurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];

                // Question girilmiş mi diye kontrol ediyor
                if (MC.Quesiton == null)
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }

                    TempData["QuestionError"] = "QuestionError";
                    TempData["ModelMCM"] = MC;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MCMSelect" });
                }
                else
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }

                    TempData["ModelMCM"] = MC;
                    TempData["QuestionError"] = "Correct";
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MCMSelect" });
                }
            }
            else if (SurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];

                if (SavedChoices == null && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "No Choices";
                }
                else if (SavedChoices == null && Choice == "" && MC.Quesiton != null)
                {
                    TempData["ChoiceError"] = "No Choices";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    TempData["ModelMCM"] = MC;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton != null)
                {
                    TempData["ModelMCM"] = MC;
                    SavedChoices = new List<string>();
                    OrderSavedChoices = new List<int>();
                    SavedChoices.Add(Choice);
                    int sequence = OrderSavedChoices.Count + 1;
                    OrderSavedChoices.Add(sequence);
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton != null)
                {
                    // dolu-boş
                    TempData["ModelMCM"] = MC;
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }

                if (MC.Quesiton == null)
                {
                    TempData["QuestionError"] = "QuestionError";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else
                {
                    Guid ThisSurvey = (Guid)TempData["SurveyID"];
                    var QuestionCategories = db.SurveyQuestionType.ToList();
                    var ThisCategory = QuestionCategories.Where(t => t.Name == "Çoktan seçmeli çoklu seçim").ToList();
                    int Sequence = SurveyQuestionNumber(ThisSurvey);

                    MC.SurveyQuestionTypeID = ThisCategory[0].Id;
                    MC.ActivityID = ThisSurvey;
                    MC.Order = Sequence + 1;

                    db.SurveyActivityQuestion.Add(MC);
                    db.SaveChanges();

                    if (Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int seq = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(seq);
                    }

                    for (int i = 0; i < SavedChoices.Count; i++)
                    {
                        SurveyQuestionOption AddThisOption = new SurveyQuestionOption();
                        AddThisOption.SurveyActivityQuestionID = MC.Id;
                        AddThisOption.Option = SavedChoices[i];
                        AddThisOption.Order = OrderSavedChoices[i];
                        db.SurveyQuestionOption.Add(AddThisOption);
                        db.SaveChanges();
                    }
                    Session["SavedChoicesMCM"] = null;
                    Session["OrderSavedChoicesMCM"] = null;
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID });
                }
            }
            return View();
        }

        public ActionResult DelChoiceMCM(string DeleteChoice, Guid? DelSurveyId)
        {
            SurveyActivityQuestion MC = (SurveyActivityQuestion)Session["ModelMCMSelect"];

            int DelThis = Convert.ToInt32(DeleteChoice);

            List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
            List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];
            OrderSavedChoices.Remove(DelThis);
            string del = SavedChoices[DelThis - 1];
            SavedChoices.Remove(del);

            for (int i = 0; i < OrderSavedChoices.Count; i++)
            {
                if (OrderSavedChoices[i] > DelThis)
                {
                    OrderSavedChoices[i] = OrderSavedChoices[i] - 1;
                }
            }


            TempData["SavedChoicesMCM"] = SavedChoices;
            TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
            Session["SavedChoicesMCM"] = SavedChoices;
            Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
            if (MC.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
            }
            else
            {
                TempData["QuestionError"] = "Correct";
            }
            TempData["ModelMCM"] = MC;

            return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = DelSurveyId, PageDisplay = "DelMCMSelect" });

        }

        public ActionResult DeleteQuestion (Guid SurveyID, Guid QuestionID)
        {
            // Question siliniyor
            SurveyActivityQuestion DeleteThis = db.SurveyActivityQuestion.Find(QuestionID);
            int missingorder = DeleteThis.Order;
            Guid QType = DeleteThis.SurveyQuestionTypeID;
            db.SurveyActivityQuestion.Remove(DeleteThis);
            db.SaveChanges();

            // Order düzenlemesi yapılıyor
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var thisquestions = AllQuestions.Where(k => k.ActivityID == SurveyID).ToList();
            for (int i = 0; i < thisquestions.Count; i++)
            {
                SurveyActivityQuestion ChangeThis = db.SurveyActivityQuestion.Find(thisquestions[i].Id);
                if (ChangeThis.Order > missingorder)
                {
                    ChangeThis.Order = ChangeThis.Order - 1;
                    db.SaveChanges();
                }
            }

            // option varsa onlar siliniyor
            var AllQuestionTypes = db.SurveyQuestionType.ToList();
            var thisQType = AllQuestionTypes.Where(l => l.Id == QType).ToList();
            if (thisQType[0].Name != "Klasik")
            {
                var AllOptions = db.SurveyQuestionOption.ToList();
                var ThisOptions = AllOptions.Where(t => t.SurveyActivityQuestionID == QuestionID).ToList();
                for (int j = 0; j < ThisOptions.Count; j++)
                {
                    SurveyQuestionOption DeleteOption = db.SurveyQuestionOption.Find(ThisOptions[j].Id);
                    db.SurveyQuestionOption.Remove(DeleteOption);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = SurveyID });
        }

        public ActionResult SurveyListEditable()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllEventActivities = db.EventActivity.ToList();
            var ThisEventActivities = AllEventActivities.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            var AllActivityCategories = db.ActivityCategory.ToList();
            var ThisActivityCategory = AllActivityCategories.Where(t => t.Name == "Anket").ToList();

            List<Activity> ThisSurveys = new List<Activity>();
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                Activity AddThis = db.Activity.Find(ThisEventActivities[i].ActivityID);

                if (AddThis.ActivityCategoryID == ThisActivityCategory[0].Id && AddThis.IsPostEventSurvey == false)
                {
                    if (AddThis.EndDate == null)
                    {
                        AddThis.EndDate = "31 December 2030 - 00:00";
                    }
                    double unixTime = SortingActivityTime(AddThis.EndDate);
                    AddThis.StartDate = unixTime.ToString();
                    ThisSurveys.Add(AddThis);
                }
            }

            List<Activity> SortedSurveyList = new List<Activity>();
            SortedSurveyList = ThisSurveys.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();

            List<EventActivity> ThisEventSurvey = new List<EventActivity>();
            for (int j = 0; j < SortedSurveyList.Count; j++)
            {
                var AddEA = ThisEventActivities.Where(l => l.ActivityID == SortedSurveyList[j].Id).ToList();
                EventActivity EA = db.EventActivity.Find(AddEA[0].Id);
                ThisEventSurvey.Add(EA);
            }

            ViewData["SurveyList"] = SortedSurveyList;
            ViewData["EventSurveyList"] = ThisEventSurvey;
            return View();
        }

        public double SortingActivityTime(String EndDate)
        {
            string sdate = EndDate.Replace(" January ", "-01-").Replace(" February ", "-02-").Replace(" March ", "-03-").Replace(" April ", "-04-").Replace(" May ", "-05-").Replace(" June ", "-06-").Replace(" July ", "-07-").Replace(" August ", "-08-").Replace(" September ", "-09-").Replace(" October ", "-10-").Replace(" November ", "-11-").Replace(" December ", "-12-");
            DateTime startdate = DateTime.ParseExact(sdate, "dd-MM-yyyy - HH:mm", new CultureInfo("tr"));
            TimeSpan span = (startdate - new DateTime(2016, 1, 1, 0, 0, 0));

            return (double)(span.TotalMinutes);
        }

        public ActionResult PublishSurvey(Guid SurveyEventID)
        {
            EventActivity publish = db.EventActivity.Find(SurveyEventID);
            publish.IsActive = true;
            db.Entry(publish).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SurveyListEditable", "Activities");
        }

       public ActionResult UnscrambleSurvey(Guid SurveyEventID)
        {
            EventActivity Unscramble = db.EventActivity.Find(SurveyEventID);
            Unscramble.IsActive = false;
            db.Entry(Unscramble).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SurveyListEditable", "Activities");
        }

        public ActionResult DeleteSurveyPermanently(Guid SurveyEventID)
        {
            // Delete EventActivity
            EventActivity DeleteThisEA = db.EventActivity.Find(SurveyEventID);
            Guid ThisActivityID = DeleteThisEA.ActivityID;
            db.EventActivity.Remove(DeleteThisEA);
            db.SaveChanges();

            // Delete Activity
            Activity DelAct = db.Activity.Find(ThisActivityID);
            db.Activity.Remove(DelAct);
            db.SaveChanges();

            // Delete Question
            List<Guid> Questions = new List<Guid>();
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var ThisQuestions = AllQuestions.Where(k => k.ActivityID == ThisActivityID).ToList();
            for (int i = 0; i < ThisQuestions.Count; i++)
            {
                Questions.Add(ThisQuestions[i].Id);
                SurveyActivityQuestion DelQ = db.SurveyActivityQuestion.Find(ThisQuestions[i].Id);
                db.SurveyActivityQuestion.Remove(DelQ);
                db.SaveChanges();
            }

            // Delete QuestionOption
            for (int j = 0; j < Questions.Count; j++)
            {
                var AllOptions = db.SurveyQuestionOption.ToList();
                var Options = AllOptions.Where(k => k.SurveyActivityQuestionID == Questions[j]).ToList();
                if (Options.Count > 0)
                {
                    for (int t = 0; t < Options.Count; t++)
                    {
                        SurveyQuestionOption DelO = db.SurveyQuestionOption.Find(Options[t].Id);
                        db.SurveyQuestionOption.Remove(DelO);
                        db.SaveChanges();
                    }
                } 
            }

            // Delete QuestionAnswer
            for (int l = 0; l < Questions.Count; l++)
            {
                var AllAnswers = db.SurveyQuestionAnswer.ToList();
                var thisAnswers = AllAnswers.Where(c => c.SurveyActivityQuestionID == Questions[l]).ToList();
                if (thisAnswers.Count > 0)
                {
                    for (int i = 0; i < thisAnswers.Count; i++)
                    {
                        SurveyQuestionAnswer Del = db.SurveyQuestionAnswer.Find(thisAnswers[i].Id);
                        db.SurveyQuestionAnswer.Remove(Del);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("SurveyListEditable", "Activities");
        }

        public ActionResult EditSurvey(Guid SurveyEventID, string DateError)
        {
            if (DateError != null)
            {
                ViewData["DateError"] = "DateError";
            }
            else
            {
                ViewData["DateError"] = "Correct";
            }
            EventActivity ThisSurveyEvent = db.EventActivity.Find(SurveyEventID);
            Activity ThisSurvey = db.Activity.Find(ThisSurveyEvent.ActivityID);

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == IDofEvent[0].Id)
            {
                ViewData["RelatedActivity"] = IDofEvent[0].Name;
            }
            else
            {
                Activity Related = db.Activity.Find(ThisSurvey.EventOrEventActivityForSurveyAndDiscussion);
                ViewData["RelatedActivity"] = Related.Name;
            }

            ViewData["ThisSurveyEvent"] = ThisSurveyEvent;
            ViewData["ThisSurvey"] = ThisSurvey;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];

            if (ThisSurvey.EndDate != null)
            {
                string DateDisplay = ThisSurvey.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["DateDisplay"] = DateDisplay;
            }
            else
            {
                ViewData["DateDisplay"] = "-";
            }

            int aaa = SurveyInfoEdit(ThisSurvey.Id);
            ViewData["EventSelected"] = TempData["EventSelected"];
            ViewData["SurveyActivityID"] = TempData["SurveyActivityID"];
            ViewData["EventInfo"] = TempData["EventInfo"];
            ViewData["SurveyName"] = TempData["SurveyName"];
            ViewData["SurveyID"] = TempData["SurveyID"];
            ViewData["RealtedActivity"] = TempData["RealtedActivity"];
            ViewData["RegisteredActivities"] = TempData["RegisteredActivities"];

            return View(ThisSurvey);
        }

        // Kullanılmıyor artık
        public ActionResult EditSurveyPage(Guid SurveyID, string DateError)
        {
            if (DateError == null)
            {
                ViewData["DateError"] = "Correct";
            }
            else
            {
                ViewData["DateError"] = "DateError";
            }
            Activity ThisSurvey = db.Activity.Find(SurveyID);
            List<Activity> RegisteredActivities = ActivityList();
            ViewData["RegisteredActivities"] = RegisteredActivities;
            Event ThisEvent = (Event)TempData["EventInfo"];
            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == ThisEvent.Id)
            {
                ViewData["EventSelected"] = "Event";
            }
            else
            {
                ViewData["EventSelected"] = "Activity";
            }

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyActivityID"] = thisEA[0].Id;

            ViewData["EventInfo"] = ThisEvent;
            ViewData["SurveyName"] = ThisSurvey.Name;
            ViewData["SurveyID"] = ThisSurvey.Id;
            ViewData["RealtedActivity"] = ThisSurvey.EventOrEventActivityForSurveyAndDiscussion;
            return View(ThisSurvey);
        }

        [HttpPost]
        public ActionResult EditSurveyInfo(Activity ThisSurvey, Guid SelectedActivity)
        {
            Guid SurveyID = (Guid)TempData["SurveyID"];
            Activity EditThis = db.Activity.Find(SurveyID);
            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == EditThis.Id).ToList();

            if (ThisSurvey.Name != null)
            {
                EditThis.Name = ThisSurvey.Name;
            }
            if (ThisSurvey.Content != null)
            {
                EditThis.Content = ThisSurvey.Content;
            }
            EditThis.EventOrEventActivityForSurveyAndDiscussion = SelectedActivity;
            db.Entry(EditThis).State = EntityState.Modified;
            db.SaveChanges();

            int datecheck = 0;
            if (ThisSurvey.EndDate != null)
            {
                String currenttime = DateTime.Now.ToLongTimeString();
                String currentday = DateTime.Now.ToLongDateString();

                string[] timedivide = currenttime.Split(new[] { " " }, StringSplitOptions.None);

                string currenthour = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }

                string currentdatetime = currentday + " " + currenthour;
                DateTime CurrentTime = Convert.ToDateTime(currentdatetime);

                string day = ThisSurvey.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                string[] dayandtime = day.Split(new[] { " - " }, StringSplitOptions.None);
                string[] daydivide = dayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[0];

                string hournormal = dayandtime[1];
                ThisSurvey.EndDate = daynormal + " " + hournormal;
                DateTime enddate = Convert.ToDateTime(ThisSurvey.EndDate);

                datecheck = DateTime.Compare(CurrentTime, enddate);

                if (datecheck >= 0)
                {
                    ViewData["DateError"] = "DateError";
                    return RedirectToAction("EditSurvey", "Activities", new { SurveyEventID = thisEA[0].Id, DateError = "DateError"});
                }
                else
                {
                    DateTime edate = Convert.ToDateTime(ThisSurvey.EndDate);
                    EditThis.EndDateTime = edate;

                    string changedate = ThisSurvey.EndDate;
                    string[] divide = changedate.Split(new[] { "-" }, StringSplitOptions.None);
                    string[] divide2 = divide[2].Split(new[] { " " }, StringSplitOptions.None);
                    string month1 = "*" + divide[1] + "*";
                    string month = month1.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
                    EditThis.EndDate = divide2[0] + " " + month + " " + divide[0] + " - " + divide2[1];
                    db.Entry(EditThis).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
                        
            return RedirectToAction("EditSurvey", "Activities", new { SurveyEventID = thisEA[0].Id });
        }

        public ActionResult DelFromEdit(Guid SurveyID, Guid QuestionID)
        {
            // Question siliniyor
            SurveyActivityQuestion DeleteThis = db.SurveyActivityQuestion.Find(QuestionID);
            int missingorder = DeleteThis.Order;
            Guid QType = DeleteThis.SurveyQuestionTypeID;
            db.SurveyActivityQuestion.Remove(DeleteThis);
            db.SaveChanges();

            // Order düzenlemesi yapılıyor
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var thisquestions = AllQuestions.Where(k => k.ActivityID == SurveyID).ToList();
            for (int i = 0; i < thisquestions.Count; i++)
            {
                SurveyActivityQuestion ChangeThis = db.SurveyActivityQuestion.Find(thisquestions[i].Id);
                if (ChangeThis.Order > missingorder)
                {
                    ChangeThis.Order = ChangeThis.Order - 1;
                    db.SaveChanges();
                }
            }

            // option varsa onlar siliniyor
            var AllQuestionTypes = db.SurveyQuestionType.ToList();
            var thisQType = AllQuestionTypes.Where(l => l.Id == QType).ToList();
            if (thisQType[0].Name != "Klasik")
            {
                var AllOptions = db.SurveyQuestionOption.ToList();
                var ThisOptions = AllOptions.Where(t => t.SurveyActivityQuestionID == QuestionID).ToList();
                for (int j = 0; j < ThisOptions.Count; j++)
                {
                    SurveyQuestionOption DeleteOption = db.SurveyQuestionOption.Find(ThisOptions[j].Id);
                    db.SurveyQuestionOption.Remove(DeleteOption);
                    db.SaveChanges();
                }
            }

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == SurveyID).ToList();

            return RedirectToAction("EditSurvey", "Activities", new { SurveyEventID = thisEA[0].Id });
        }

        public ActionResult EditMC(Guid? QQQ, string OptionError, string MCFrom)
        {
            if (MCFrom != null)
            {
                Session["MCFrom"] = MCFrom;
            }
            
            if (OptionError == "OptionError")
            {
                ViewData["OptionError"] = "OptionError";
            }
            else
            {
                ViewData["OptionError"] = "Correct";
            }

            var AllOptions = db.SurveyQuestionOption.ToList();
            var ThisOptions = AllOptions.Where(k => k.SurveyActivityQuestionID == QQQ).ToList();
            List<SurveyQuestionOption> SavedOptions = new List<SurveyQuestionOption>();
            if (ThisOptions.Count > 0)
            {
                for (int i = 0; i < ThisOptions.Count; i++)
                {
                    SurveyQuestionOption AddThis = db.SurveyQuestionOption.Find(ThisOptions[i].Id);
                    SavedOptions.Add(AddThis);
                }
            }

            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QQQ);

            List<SurveyQuestionOption> SortedOptions = new List<SurveyQuestionOption>();
            SortedOptions = SavedOptions.OrderByDescending(q => q.Order).ToList();

            ViewData["Options"] = SortedOptions;

            var AllActivities = db.Activity.ToList();
            var Survey = AllActivities.Where(k => k.Id == ThisQuestion.ActivityID).ToList();
            Activity ThisSurvey = db.Activity.Find(Survey[0].Id);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyActivityID"] = thisEA[0].Id;
            ViewData["QID"] = ThisQuestion.Id;
            ViewData["ActivityID"] = ThisSurvey.Id;

            return View(ThisQuestion);
        }

        public ActionResult DeleteChoice(Guid OptionID)
        {
            Guid SurveyID = (Guid)TempData["SurveyID"];
            SurveyQuestionOption ThisOption = db.SurveyQuestionOption.Find(OptionID);
            int OptionOrder = ThisOption.Order;
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(ThisOption.SurveyActivityQuestionID);

            db.SurveyQuestionOption.Remove(ThisOption);
            db.SaveChanges();

            var AllChoices = db.SurveyQuestionOption.ToList();
            var QChoices = AllChoices.Where(k => k.SurveyActivityQuestionID == ThisQuestion.Id).ToList();
            List<SurveyQuestionOption> QuestionOptions = new List<SurveyQuestionOption>();
            for (int i = 0; i < QChoices.Count; i++)
            {
                SurveyQuestionOption AddThis = db.SurveyQuestionOption.Find(QChoices[i].Id);
                QuestionOptions.Add(AddThis);
            }

            for (int j = 0; j < QuestionOptions.Count; j++)
            {
                if (QuestionOptions[j].Order > OptionOrder)
                {
                    QuestionOptions[j].Order = QuestionOptions[j].Order - 1;
                    db.Entry(QuestionOptions[j]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("EditMC", "Activities", new { QQQ = ThisQuestion.Id });
        }

        [HttpPost]
        public ActionResult AddChoice(string Complete, SurveyActivityQuestion QQQ, string Choice)
        {
            string from = (string)Session["MCFrom"];
            Guid QuestionID = (Guid)TempData["QuestionID"];
            var AllChoices = db.SurveyQuestionOption.ToList();
            var ThisOptions = AllChoices.Where(k => k.SurveyActivityQuestionID == QuestionID).ToList();
            int CountOption = ThisOptions.Count;

            if (Choice != "")
            {
                SurveyQuestionOption NewOption = new SurveyQuestionOption();
                NewOption.Order = CountOption + 1;
                NewOption.Option = Choice;
                NewOption.SurveyActivityQuestionID = QuestionID;
                db.SurveyQuestionOption.Add(NewOption);
                db.SaveChanges();
            }

            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QuestionID);
            if (QQQ.Quesiton != null)
            {
                ThisQuestion.Quesiton = QQQ.Quesiton;
                db.Entry(ThisQuestion).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (Complete == "Complete")
            {
                var Opts = AllChoices.Where(k => k.SurveyActivityQuestionID == QuestionID).ToList();
                int Count = Opts.Count;
                if (Count <= 1)
                {
                    return RedirectToAction("EditMC", "Activities", new { QQQ = ThisQuestion.Id, OptionError = "OptionError" });
                }

                Guid ActivityID = (Guid)TempData["ActivityID"];
                var AllEA = db.EventActivity.ToList();
                var thisEA = AllEA.Where(k => k.ActivityID == ActivityID).ToList();

                if (from == "CreateSurveyDetails")
                {
                    return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ActivityID });
                }
                else if (from == "EditSurvey")
                {
                    return RedirectToAction("EditSurvey", "Activities", new { SurveyEventID = thisEA[0].Id });
                }
            }

            return RedirectToAction("EditMC", "Activities", new { QQQ = ThisQuestion.Id, MCFrom = from });
        }

        public ActionResult EditClassic(Guid QQQ, string EditQFrom)
        {
            Session["EditQFrom"] = EditQFrom;
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QQQ);

            var AllActivities = db.Activity.ToList();
            var Survey = AllActivities.Where(k => k.Id == ThisQuestion.ActivityID).ToList();
            Activity ThisSurvey = db.Activity.Find(Survey[0].Id);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyActivityID"] = thisEA[0].Id;
            ViewData["QID"] = ThisQuestion.Id;

            return View(ThisQuestion);
        }

        [HttpPost]
        public ActionResult SaveCL(SurveyActivityQuestion QQQ)
        {
            Guid QuestionID = (Guid)TempData["QuestionID"];
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QuestionID);
            if (QQQ.Quesiton != null)
            {
                ThisQuestion.Quesiton = QQQ.Quesiton;
                db.Entry(ThisQuestion).State = EntityState.Modified;
                db.SaveChanges();
            }

            Guid ActivityID = (Guid)TempData["ActivityID"];
            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ActivityID).ToList();

            string EditQFrom = (string)Session["EditQFrom"];
            if (EditQFrom == "CreateSurveyDetails")
            {
                return RedirectToAction("CreateSurveyDetails", "Activities", new { thissurvey = ActivityID});
            }
            else if(EditQFrom == "EditSurvey")
            {
                return RedirectToAction("EditSurvey", "Activities", new { SurveyEventID = thisEA[0].Id });
            }
            return View();
        }

        public ActionResult SurveyDisplayOrg(Guid SurveyEventID)
        {
            EventActivity ThisSurveyEvent = db.EventActivity.Find(SurveyEventID);
            Activity ThisSurvey = db.Activity.Find(ThisSurveyEvent.ActivityID);

            if (ThisSurveyEvent.IsActive == true)
            {
                ViewData["IsActive"] = "true";
            }
            else
            {
                ViewData["IsActive"] = "false";
            }

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == IDofEvent[0].Id)
            {
                ViewData["RelatedActivity"] = IDofEvent[0].Name;
            }
            else
            {
                Activity Related = db.Activity.Find(ThisSurvey.EventOrEventActivityForSurveyAndDiscussion);
                ViewData["RelatedActivity"] = Related.Name;
            }

            ViewData["ThisSurveyEvent"] = ThisSurveyEvent;
            ViewData["ThisSurvey"] = ThisSurvey;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];

            if (ThisSurvey.EndDate != null)
            {
                string DateDisplay = ThisSurvey.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["DateDisplay"] = DateDisplay;
            }
            else
            {
                ViewData["DateDisplay"] = "-";
            }

            return View();
        }

        public ActionResult CompleteSurvey(Guid SurveyEventID)
        {
            EventActivity ThisEA = db.EventActivity.Find(SurveyEventID);
            ThisEA.IsActive = true;
            db.Entry(ThisEA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SurveyDisplayOrg", "Activities", new { SurveyEventID = ThisEA.Id});
        }

        public ActionResult CancelSurvey(Guid SurveyEventID)
        {
            EventActivity ThisEA = db.EventActivity.Find(SurveyEventID);
            ThisEA.IsActive = false;
            db.Entry(ThisEA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SurveyDisplayOrg", "Activities", new { SurveyEventID = ThisEA.Id });
        }

        public ActionResult PESurveyOrg()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.EventID == IDofEvent[0].Id).ToList();
            
            var TypeOfActivity = db.ActivityCategory.ToList();
            var SurveyType = TypeOfActivity.Where(k => k.Name == "Anket").ToList();

            // PES oluşturulmuş mu oluşturulmamış mı diye bakıyor
            Activity PES = null;
            for (int i = 0; i < ThisEA.Count; i++)
            {
                Activity AddThis = db.Activity.Find(ThisEA[i].ActivityID);
                if (AddThis.ActivityCategoryID == SurveyType[0].Id && AddThis.IsPostEventSurvey == true)
                {
                    PES = AddThis;
                    ViewData["SurveyEventID"] = ThisEA[i].Id;
                }
            }

            // PES oluşturulmuş ise sorularını falan çekiyor
            if (PES != null)
            {
                string EndDate = PES.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["EndDate"] = EndDate;
                string StartDate = PES.StartDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["StartDate"] = StartDate;

                ControllerToSurvey(PES.Id);
                var PESEvent = ThisEA.Where(l => l.ActivityID == PES.Id).ToList();
                if (PESEvent[0].IsActive == true)
                {
                    ViewData["IsActive"] = "true";
                }
                else
                {
                    ViewData["IsActive"] = "false";
                }
                ViewData["EventName"] = IDofEvent[0].Name;
                ViewData["PES"] = PES;
                ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
                ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
                ViewData["QuestionOptions"] = TempData["QuestionOptions"];
                ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];
            }
            else
            {
                ViewData["EventName"] = IDofEvent[0].Name;
                ViewData["IsActive"] = "";
                ViewData["PES"] = null;
                ViewData["SortedSurveyQuestions"] = null;
                ViewData["OptionNumberOfQuestions"] = null;
                ViewData["QuestionOptions"] = null;
                ViewData["TypeOfQuestionByOrder"] = null;
                ViewData["StartDate"] = "";
                ViewData["EndDate"] = "";
                ViewData["SurveyEventID"] = Guid.Empty;
            }
            
            return View();
        }

        public ActionResult CreatePESDates()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            Event ThisEventInfo = db.Event.Find(thisEvent[0].Id);
            ViewData["EventInfo"] = ThisEventInfo;

            ViewData["StartDateError"] = "Correct";
            ViewData["EndDateError"] = "Correct";
            ViewData["DateCompareError"] = "Correct";
            ViewData["DateError"] = "Correct";

            var TypeOfActivity = db.ActivityCategory.ToList();
            var SurveyType = TypeOfActivity.Where(k => k.Name == "Anket").ToList();

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.EventID == ThisEventInfo.Id).ToList();

            Activity ThisPES = null;
            for (int i = 0; i < ThisEA.Count; i++)
            {
                Activity ThisActivity = db.Activity.Find(ThisEA[i].ActivityID);
                if (ThisActivity.ActivityCategoryID == SurveyType[0].Id && ThisActivity.IsPostEventSurvey == true)
                {
                    ThisPES = ThisActivity;
                    ViewData["PESExists"] = "PESExsists";
                    return View(ThisPES);
                }
            }

            if (ThisPES == null)
            {
                ViewData["PESExists"] = "PESnotExists";
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreatePESDates(Activity PES)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            Event ThisEventInfo = db.Event.Find(thisEvent[0].Id);
            string CompleteTime = PES.EndDate;
            
            string DateCompareError = "Correct";
            string StartDateError = "Correct";
            string EndDateError = "Correct";
            int datecheck = 0;

            // Date girilmiş mi diye kontrol ediyor
            if ((PES.StartDate != null && PES.EndDate != null))
            {
                string sdate = PES.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = PES.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                }
            }
            else if (PES.StartDate == null && PES.EndDate != null)
            {
                StartDateError = "StartDateError";
            }
            else if (PES.StartDate != null && PES.EndDate == null)
            {
                EndDateError = "EndDateError";
            }
            else
            {
                StartDateError = "StartDateError";
                EndDateError = "EndDateError";
            }

            if (StartDateError == "StartDateError" || EndDateError == "EndDateError" || DateCompareError == "DateCompareError")
            {
                ViewData["EventInfo"] = ThisEventInfo;
                ViewData["StartDateError"] = StartDateError;
                ViewData["EndDateError"] = EndDateError;
                ViewData["DateCompareError"] = DateCompareError;
                ViewData["DateError"] = "Correct";
                return View(PES);
            }

            // Başlangıç tarihinin şu andan önce olmaması kontrol ediliyor
            if (PES.StartDate != null)
            {
                String currenttime = DateTime.Now.ToLongTimeString();
                String currentday = DateTime.Now.ToLongDateString();

                string[] timedivide = currenttime.Split(new[] { " " }, StringSplitOptions.None);

                string currenthour = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }

                string currentdatetime = currentday + " " + currenthour;
                DateTime CurrentTime = Convert.ToDateTime(currentdatetime);

                string day = PES.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                string[] dayandtime = day.Split(new[] { " - " }, StringSplitOptions.None);
                string[] daydivide = dayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[0];

                string hournormal = dayandtime[1];
                PES.StartDate = daynormal + " " + hournormal;
                DateTime startdate = Convert.ToDateTime(PES.StartDate);

                datecheck = DateTime.Compare(CurrentTime, startdate);

                if (datecheck >= 0)
                {
                    ViewData["EventInfo"] = ThisEventInfo;
                    ViewData["StartDateError"] = "Correct";
                    ViewData["EndDateError"] = "Correct";
                    ViewData["DateCompareError"] = "Correct";
                    ViewData["DateError"] = "DateError";
                    return View(PES);
                }
            }

            DateTime Openning = Convert.ToDateTime(PES.StartDate);
            PES.StartDateTime = Openning;

            string changedate = PES.StartDate;
            string[] divide = changedate.Split(new[] { "-" }, StringSplitOptions.None);
            string[] divide2 = divide[2].Split(new[] { " " }, StringSplitOptions.None);
            string month1 = "*" + divide[1] + "*";
            string month = month1.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
            PES.StartDate = divide2[0] + " " + month + " " + divide[0] + " - " + divide2[1];

            // EndDate formatı düzeltiliyor
            String CCcurrenttime = DateTime.Now.ToLongTimeString();
            String CCcurrentday = DateTime.Now.ToLongDateString();

            string[] CCtimedivide = CCcurrenttime.Split(new[] { " " }, StringSplitOptions.None);

            string CCcurrenthour = "";
            if (CCtimedivide[1] == "PM")
            {
                string[] CChourdivide = CCtimedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                CChourdivide[0] = "*" + CChourdivide[0] + "*";
                CChourdivide[0] = CChourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                CCcurrenthour = CChourdivide[0] + ":" + CChourdivide[1];
            }
            else if (CCtimedivide[1] == "AM")
            {
                string[] CChourdivide = CCtimedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                CChourdivide[0] = CChourdivide[0].Replace("12", "0");
                CCcurrenthour = CChourdivide[0] + ":" + CChourdivide[1];
            }

            string CCcurrentdatetime = CCcurrentday + " " + CCcurrenthour;
            DateTime CCCurrentTime = Convert.ToDateTime(CCcurrentdatetime);

            string CCday = CompleteTime.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            string[] CCdayandtime = CCday.Split(new[] { " - " }, StringSplitOptions.None);
            string[] CCdaydivide = CCdayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
            string CCdaynormal = CCdaydivide[2] + "-" + CCdaydivide[1] + "-" + CCdaydivide[0];

            string CChournormal = CCdayandtime[1];
            PES.EndDate = CCdaynormal + " " + CChournormal;

            DateTime Complete = Convert.ToDateTime(PES.EndDate);
            PES.EndDateTime = Complete;

            string Cchangedate = PES.EndDate;
            string[] Cdivide = Cchangedate.Split(new[] { "-" }, StringSplitOptions.None);
            string[] Cdivide2 = Cdivide[2].Split(new[] { " " }, StringSplitOptions.None);
            string Cmonth1 = "*" + Cdivide[1] + "*";
            string Cmonth = Cmonth1.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
            PES.EndDate = Cdivide2[0] + " " + Cmonth + " " + Cdivide[0] + " - " + Cdivide2[1];

            // Kayıtlı PES var mı yok mu kontrol ediyor
            var TypeOfActivity = db.ActivityCategory.ToList();
            var SurveyType = TypeOfActivity.Where(k => k.Name == "Anket").ToList();

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.EventID == ThisEventInfo.Id).ToList();

            Activity ThisPES = null;
            for (int i = 0; i < ThisEA.Count; i++)
            {
                Activity ThisActivity = db.Activity.Find(ThisEA[i].ActivityID);
                if (ThisActivity.ActivityCategoryID == SurveyType[0].Id && ThisActivity.IsPostEventSurvey == true)
                {
                    ThisPES = ThisActivity;
                    ViewData["PESExists"] = "PESExsists";
                }
            }

            Session["FromPageSurvey"] = "CreateSurvey";

            if (ThisPES == null)
            {
                Activity CreatePES = new Activity();
                CreatePES.EndDate = PES.EndDate;
                CreatePES.StartDate = PES.StartDate;
                CreatePES.Name = thisEvent[0].Name + " Etkinlik Sonrası Anketi";
                CreatePES.Content = "Etkinlik Değerlendirme Anketi";
                CreatePES.ActivityCategoryID = SurveyType[0].Id;
                CreatePES.IsActive = true;
                CreatePES.EventOrEventActivityForSurveyAndDiscussion = thisEvent[0].Id;
                CreatePES.IsPostEventSurvey = true;
                CreatePES.StartDateTime = PES.StartDateTime;
                CreatePES.EndDateTime = PES.EndDateTime;
                db.Activity.Add(CreatePES);
                db.SaveChanges();

                EventActivity EA = new EventActivity();
                EA.EventID = thisEvent[0].Id;
                EA.ActivityID = CreatePES.Id;
                EA.IsActive = false;
                db.EventActivity.Add(EA);
                db.SaveChanges();
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = CreatePES.Id });
            }
            else
            {
                // burası silinecek ya da başka bir yere eklenecek
                ThisPES.StartDate = PES.StartDate;
                ThisPES.StartDateTime = PES.StartDateTime;
                ThisPES.EndDateTime = PES.EndDateTime;
                ThisPES.EndDate = PES.EndDate;
                db.Entry(ThisPES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ThisPES.Id });
                // Burada revize et
            }
        }

        public dynamic SurveyInfoEdit(Guid SurveyID)
        {
            Activity ThisSurvey = db.Activity.Find(SurveyID);
            List<Activity> RegisteredActivities = ActivityList();
            TempData["RegisteredActivities"] = RegisteredActivities;
            Event ThisEvent = (Event)TempData["EventInfo"];
            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == ThisEvent.Id)
            {
                TempData["EventSelected"] = "Event";
            }
            else
            {
                TempData["EventSelected"] = "Activity";
            }

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            TempData["SurveyActivityID"] = thisEA[0].Id;

            TempData["EventInfo"] = ThisEvent;
            TempData["SurveyName"] = ThisSurvey.Name;
            TempData["SurveyID"] = ThisSurvey.Id;
            TempData["RealtedActivity"] = ThisSurvey.EventOrEventActivityForSurveyAndDiscussion;
            return 111;
        }

        public ActionResult CreatePESDetails(Guid thissurvey, string PageDisplay)
        {
            Session["FromPageSurvey"] = (string)Session["FromPageSurvey"];

            Activity ThisSurvey = db.Activity.Find(thissurvey);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyEventID"] = thisEA[0].Id;

            List<SurveyQuestionType> QuestionTypes = db.SurveyQuestionType.ToList();
            ViewData["QuesitonTypes"] = QuestionTypes;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];


            ViewData["ChoiceError"] = "Correct";

            if (PageDisplay == null)
            {
                ViewData["PageDisplay"] = "Default";
            }
            else if (PageDisplay == "Classic")
            {
                ViewData["PageDisplay"] = "Classic";
                ViewData["QuestionError"] = "QuestionError";
            }
            else if (PageDisplay == "MC1Select")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MC1Select";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoices"] = TempData["SavedChoices"];
                ViewData["OrderSavedChoices"] = TempData["OrderSavedChoices"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["Model"];
                return View(Model);
            }
            else if (PageDisplay == "MCMSelect")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MCMSelect";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoicesMCM"] = TempData["SavedChoicesMCM"];
                ViewData["OrderSavedChoicesMCM"] = TempData["OrderSavedChoicesMCM"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["ModelMCM"];
                return View(Model);
            }
            else if (PageDisplay == "DelMC1Select")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MC1Select";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoices"] = TempData["SavedChoices"];
                ViewData["OrderSavedChoices"] = TempData["OrderSavedChoices"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["Model"];
                return View(Model);
            }
            else if (PageDisplay == "DelMCMSelect")
            {
                string ChoiceError = (string)TempData["ChoiceError"];
                if (ChoiceError != null)
                {
                    ViewData["ChoiceError"] = ChoiceError;
                }
                else
                {
                    ViewData["ChoiceError"] = "Correct";
                }
                ViewData["PageDisplay"] = "MCMSelect";
                ViewData["QuestionError"] = TempData["QuestionError"];
                ViewData["SavedChoicesMCM"] = TempData["SavedChoicesMCM"];
                ViewData["OrderSavedChoicesMCM"] = TempData["OrderSavedChoicesMCM"];

                SurveyActivityQuestion Model = (SurveyActivityQuestion)TempData["ModelMCM"];
                return View(Model);
            }

            return View();
        }

        public ActionResult SaveClassicPES(Guid SurveyID, SurveyActivityQuestion QQQ)
        {
            if (QQQ.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "Classic" });
            }
            var QuestionCategories = db.SurveyQuestionType.ToList();
            var ThisCategory = QuestionCategories.Where(t => t.Name == "Klasik").ToList();

            int Sequence = SurveyQuestionNumber(SurveyID);

            QQQ.SurveyQuestionTypeID = ThisCategory[0].Id;
            QQQ.ActivityID = SurveyID;
            QQQ.Order = Sequence + 1;

            db.SurveyActivityQuestion.Add(QQQ);
            db.SaveChanges();

            return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID });
        }

        public ActionResult CancelClassicPES()
        {
            Guid CancelID = (Guid)TempData["CancelClassic"];
            return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = CancelID });
        }

        public ActionResult SaveMC1SelectPES(Guid? SurveyID, Guid? ContSurveyID, Guid? DelSurveyId, Guid? CancelSurveyID, string Choice, SurveyActivityQuestion MC)
        {
            if (CancelSurveyID != null)
            {
                Session["SavedChoices"] = null;
                Session["OrderSavedChoices"] = null;
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = CancelSurveyID });
            }
            Session["ModelMC1"] = MC;
            if (ContSurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoices"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];

                // Question girilmiş mi diye kontrol ediyor
                if (MC.Quesiton == null)
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }

                    TempData["QuestionError"] = "QuestionError";
                    TempData["Model"] = MC;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MC1Select" });
                }
                else
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoices"] = SavedChoices;
                        TempData["OrderSavedChoices"] = OrderSavedChoices;
                        Session["SavedChoices"] = SavedChoices;
                        Session["OrderSavedChoices"] = OrderSavedChoices;
                    }

                    TempData["Model"] = MC;
                    TempData["QuestionError"] = "Correct";
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MC1Select" });
                }
            }
            else if (SurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoices"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];

                if (SavedChoices == null && Choice == "" && MC.Quesiton == null)
                {
                    // boş-boş
                    TempData["ChoiceError"] = "No Choices";
                }
                else if (SavedChoices == null && Choice == "" && MC.Quesiton != null)
                {
                    // boş-boş
                    TempData["ChoiceError"] = "No Choices";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    TempData["Model"] = MC;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton == null)
                {
                    // boş-dolu
                    TempData["ChoiceError"] = "Only Choice";
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton != null)
                {
                    // boş-dolu
                    TempData["Model"] = MC;
                    SavedChoices = new List<string>();
                    OrderSavedChoices = new List<int>();
                    SavedChoices.Add(Choice);
                    int sequence = OrderSavedChoices.Count + 1;
                    OrderSavedChoices.Add(sequence);
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton != null)
                {
                    // dolu-boş
                    TempData["Model"] = MC;
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }

                if (MC.Quesiton == null)
                {
                    TempData["QuestionError"] = "QuestionError";
                    TempData["SavedChoices"] = SavedChoices;
                    TempData["OrderSavedChoices"] = OrderSavedChoices;
                    Session["SavedChoices"] = SavedChoices;
                    Session["OrderSavedChoices"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MC1Select" });
                }
                else
                {
                    Guid ThisSurvey = (Guid)TempData["SurveyID"];
                    var QuestionCategories = db.SurveyQuestionType.ToList();
                    var ThisCategory = QuestionCategories.Where(t => t.Name == "Çoktan seçmeli tek seçim").ToList();
                    int Sequence = SurveyQuestionNumber(ThisSurvey);

                    MC.SurveyQuestionTypeID = ThisCategory[0].Id;
                    MC.ActivityID = ThisSurvey;
                    MC.Order = Sequence + 1;

                    db.SurveyActivityQuestion.Add(MC);
                    db.SaveChanges();

                    if (Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int seq = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(seq);
                    }

                    for (int i = 0; i < SavedChoices.Count; i++)
                    {
                        SurveyQuestionOption AddThisOption = new SurveyQuestionOption();
                        AddThisOption.SurveyActivityQuestionID = MC.Id;
                        AddThisOption.Option = SavedChoices[i];
                        AddThisOption.Order = OrderSavedChoices[i];
                        db.SurveyQuestionOption.Add(AddThisOption);
                        db.SaveChanges();
                    }
                    Session["SavedChoices"] = null;
                    Session["OrderSavedChoices"] = null;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID });
                }
            }
            return View();
        }

        public ActionResult DelChoiceMC1PES(string DeleteChoice, Guid? DelSurveyId)
        {
            SurveyActivityQuestion MC = (SurveyActivityQuestion)Session["ModelMC1"];

            int DelThis = Convert.ToInt32(DeleteChoice);

            List<string> SavedChoices = (List<string>)Session["SavedChoices"];
            List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoices"];
            OrderSavedChoices.Remove(DelThis);
            string del = SavedChoices[DelThis - 1];
            SavedChoices.Remove(del);

            for (int i = 0; i < OrderSavedChoices.Count; i++)
            {
                if (OrderSavedChoices[i] > DelThis)
                {
                    OrderSavedChoices[i] = OrderSavedChoices[i] - 1;
                }
            }


            TempData["SavedChoices"] = SavedChoices;
            TempData["OrderSavedChoices"] = OrderSavedChoices;
            Session["SavedChoices"] = SavedChoices;
            Session["OrderSavedChoices"] = OrderSavedChoices;
            if (MC.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
            }
            else
            {
                TempData["QuestionError"] = "Correct";
            }
            TempData["Model"] = MC;

            return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = DelSurveyId, PageDisplay = "DelMC1Select" });

        }

        public ActionResult SaveMCMSelectPES(Guid? SurveyID, Guid? ContSurveyID, Guid? DelSurveyId, Guid? CancelSurveyID, string Choice, SurveyActivityQuestion MC)
        {
            if (CancelSurveyID != null)
            {
                Session["SavedChoicesMCM"] = null;
                Session["OrderSavedChoicesMCM"] = null;
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = CancelSurveyID });
            }
            Session["ModelMCMSelect"] = MC;
            if (ContSurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];

                // Question girilmiş mi diye kontrol ediyor
                if (MC.Quesiton == null)
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }

                    TempData["QuestionError"] = "QuestionError";
                    TempData["ModelMCM"] = MC;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MCMSelect" });
                }
                else
                {
                    if (SavedChoices == null && Choice != "")
                    {
                        SavedChoices = new List<string>();
                        OrderSavedChoices = new List<int>();

                        SavedChoices.Add(Choice);
                        OrderSavedChoices.Add(1);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else if (SavedChoices != null && Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int sequence = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(sequence);
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }
                    else
                    {
                        TempData["SavedChoicesMCM"] = SavedChoices;
                        TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                        Session["SavedChoicesMCM"] = SavedChoices;
                        Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    }

                    TempData["ModelMCM"] = MC;
                    TempData["QuestionError"] = "Correct";
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ContSurveyID, PageDisplay = "MCMSelect" });
                }
            }
            else if (SurveyID != null)
            {
                List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
                List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];

                if (SavedChoices == null && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "No Choices";
                }
                else if (SavedChoices == null && Choice == "" && MC.Quesiton != null)
                {
                    TempData["ChoiceError"] = "No Choices";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    TempData["ModelMCM"] = MC;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }
                else if (SavedChoices == null && Choice != "" && MC.Quesiton != null)
                {
                    TempData["ModelMCM"] = MC;
                    SavedChoices = new List<string>();
                    OrderSavedChoices = new List<int>();
                    SavedChoices.Add(Choice);
                    int sequence = OrderSavedChoices.Count + 1;
                    OrderSavedChoices.Add(sequence);
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton != null)
                {
                    // dolu-boş
                    TempData["ModelMCM"] = MC;
                    TempData["ChoiceError"] = "Only Choice";
                    TempData["QuestionError"] = "Correct";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else if (SavedChoices.Count == 1 && Choice == "" && MC.Quesiton == null)
                {
                    TempData["ChoiceError"] = "Only Choice";
                }

                if (MC.Quesiton == null)
                {
                    TempData["QuestionError"] = "QuestionError";
                    TempData["SavedChoicesMCM"] = SavedChoices;
                    TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    Session["SavedChoicesMCM"] = SavedChoices;
                    Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID, PageDisplay = "MCMSelect" });
                }
                else
                {
                    Guid ThisSurvey = (Guid)TempData["SurveyID"];
                    var QuestionCategories = db.SurveyQuestionType.ToList();
                    var ThisCategory = QuestionCategories.Where(t => t.Name == "Çoktan seçmeli çoklu seçim").ToList();
                    int Sequence = SurveyQuestionNumber(ThisSurvey);

                    MC.SurveyQuestionTypeID = ThisCategory[0].Id;
                    MC.ActivityID = ThisSurvey;
                    MC.Order = Sequence + 1;

                    db.SurveyActivityQuestion.Add(MC);
                    db.SaveChanges();

                    if (Choice != "")
                    {
                        SavedChoices.Add(Choice);
                        int seq = OrderSavedChoices.Count + 1;
                        OrderSavedChoices.Add(seq);
                    }

                    for (int i = 0; i < SavedChoices.Count; i++)
                    {
                        SurveyQuestionOption AddThisOption = new SurveyQuestionOption();
                        AddThisOption.SurveyActivityQuestionID = MC.Id;
                        AddThisOption.Option = SavedChoices[i];
                        AddThisOption.Order = OrderSavedChoices[i];
                        db.SurveyQuestionOption.Add(AddThisOption);
                        db.SaveChanges();
                    }
                    Session["SavedChoicesMCM"] = null;
                    Session["OrderSavedChoicesMCM"] = null;
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID });
                }
            }
            return View();
        }

        public ActionResult DelChoiceMCMPES(string DeleteChoice, Guid? DelSurveyId)
        {
            SurveyActivityQuestion MC = (SurveyActivityQuestion)Session["ModelMCMSelect"];

            int DelThis = Convert.ToInt32(DeleteChoice);

            List<string> SavedChoices = (List<string>)Session["SavedChoicesMCM"];
            List<int> OrderSavedChoices = (List<int>)Session["OrderSavedChoicesMCM"];
            OrderSavedChoices.Remove(DelThis);
            string del = SavedChoices[DelThis - 1];
            SavedChoices.Remove(del);

            for (int i = 0; i < OrderSavedChoices.Count; i++)
            {
                if (OrderSavedChoices[i] > DelThis)
                {
                    OrderSavedChoices[i] = OrderSavedChoices[i] - 1;
                }
            }


            TempData["SavedChoicesMCM"] = SavedChoices;
            TempData["OrderSavedChoicesMCM"] = OrderSavedChoices;
            Session["SavedChoicesMCM"] = SavedChoices;
            Session["OrderSavedChoicesMCM"] = OrderSavedChoices;
            if (MC.Quesiton == null)
            {
                TempData["QuestionError"] = "QuestionError";
            }
            else
            {
                TempData["QuestionError"] = "Correct";
            }
            TempData["ModelMCM"] = MC;

            return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = DelSurveyId, PageDisplay = "DelMCMSelect" });

        }

        public ActionResult DeleteQuestionPES(Guid SurveyID, Guid QuestionID)
        {
            // Question siliniyor
            SurveyActivityQuestion DeleteThis = db.SurveyActivityQuestion.Find(QuestionID);
            int missingorder = DeleteThis.Order;
            Guid QType = DeleteThis.SurveyQuestionTypeID;
            db.SurveyActivityQuestion.Remove(DeleteThis);
            db.SaveChanges();

            // Order düzenlemesi yapılıyor
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var thisquestions = AllQuestions.Where(k => k.ActivityID == SurveyID).ToList();
            for (int i = 0; i < thisquestions.Count; i++)
            {
                SurveyActivityQuestion ChangeThis = db.SurveyActivityQuestion.Find(thisquestions[i].Id);
                if (ChangeThis.Order > missingorder)
                {
                    ChangeThis.Order = ChangeThis.Order - 1;
                    db.SaveChanges();
                }
            }

            // option varsa onlar siliniyor
            var AllQuestionTypes = db.SurveyQuestionType.ToList();
            var thisQType = AllQuestionTypes.Where(l => l.Id == QType).ToList();
            if (thisQType[0].Name != "Klasik")
            {
                var AllOptions = db.SurveyQuestionOption.ToList();
                var ThisOptions = AllOptions.Where(t => t.SurveyActivityQuestionID == QuestionID).ToList();
                for (int j = 0; j < ThisOptions.Count; j++)
                {
                    SurveyQuestionOption DeleteOption = db.SurveyQuestionOption.Find(ThisOptions[j].Id);
                    db.SurveyQuestionOption.Remove(DeleteOption);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = SurveyID });
        }

        public ActionResult EditClassicPES(Guid QQQ, string EditQFrom)
        {
            Session["EditQFrom"] = EditQFrom;
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QQQ);

            var AllActivities = db.Activity.ToList();
            var Survey = AllActivities.Where(k => k.Id == ThisQuestion.ActivityID).ToList();
            Activity ThisSurvey = db.Activity.Find(Survey[0].Id);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyActivityID"] = thisEA[0].Id;
            ViewData["QID"] = ThisQuestion.Id;

            return View(ThisQuestion);
        }

        public ActionResult SaveCLPES(SurveyActivityQuestion QQQ)
        {
            Guid QuestionID = (Guid)TempData["QuestionID"];
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QuestionID);
            if (QQQ.Quesiton != null)
            {
                ThisQuestion.Quesiton = QQQ.Quesiton;
                db.Entry(ThisQuestion).State = EntityState.Modified;
                db.SaveChanges();
            }

            Guid ActivityID = (Guid)TempData["ActivityID"];
            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ActivityID).ToList();

            string EditQFrom = (string)Session["EditQFrom"];
            if (EditQFrom == "CreateSurveyDetails")
            {
                return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ActivityID });
            }
            else if (EditQFrom == "EditSurvey")
            {
                return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id });
            }
            return View();
        }

        public ActionResult EditMCPES(Guid? QQQ, string OptionError, string MCFrom)
        {
            if (MCFrom != null)
            {
                Session["MCFrom"] = MCFrom;
            }

            if (OptionError == "OptionError")
            {
                ViewData["OptionError"] = "OptionError";
            }
            else
            {
                ViewData["OptionError"] = "Correct";
            }

            var AllOptions = db.SurveyQuestionOption.ToList();
            var ThisOptions = AllOptions.Where(k => k.SurveyActivityQuestionID == QQQ).ToList();
            List<SurveyQuestionOption> SavedOptions = new List<SurveyQuestionOption>();
            if (ThisOptions.Count > 0)
            {
                for (int i = 0; i < ThisOptions.Count; i++)
                {
                    SurveyQuestionOption AddThis = db.SurveyQuestionOption.Find(ThisOptions[i].Id);
                    SavedOptions.Add(AddThis);
                }
            }

            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QQQ);

            List<SurveyQuestionOption> SortedOptions = new List<SurveyQuestionOption>();
            SortedOptions = SavedOptions.OrderByDescending(q => q.Order).ToList();

            ViewData["Options"] = SortedOptions;

            var AllActivities = db.Activity.ToList();
            var Survey = AllActivities.Where(k => k.Id == ThisQuestion.ActivityID).ToList();
            Activity ThisSurvey = db.Activity.Find(Survey[0].Id);
            ViewData["ThisSurvey"] = ThisSurvey;

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == ThisSurvey.Id).ToList();
            ViewData["SurveyActivityID"] = thisEA[0].Id;
            ViewData["QID"] = ThisQuestion.Id;
            ViewData["ActivityID"] = ThisSurvey.Id;

            return View(ThisQuestion);
        }

        public ActionResult EditPES(Guid SurveyEventID, string DateError, string DateCompareError)
        {
            if (DateError != null)
            {
                ViewData["DateError"] = "DateError";
            }
            else
            {
                ViewData["DateError"] = "Correct";
            }

            if (DateCompareError != null)
            {
                ViewData["DateCompareError"] = "DateCompareError";
            }
            else
            {
                ViewData["DateCompareError"] = "Correct";
            }


            EventActivity ThisSurveyEvent = db.EventActivity.Find(SurveyEventID);
            Activity ThisSurvey = db.Activity.Find(ThisSurveyEvent.ActivityID);

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == IDofEvent[0].Id)
            {
                ViewData["RelatedActivity"] = IDofEvent[0].Name;
            }
            else
            {
                Activity Related = db.Activity.Find(ThisSurvey.EventOrEventActivityForSurveyAndDiscussion);
                ViewData["RelatedActivity"] = Related.Name;
            }

            ViewData["ThisSurveyEvent"] = ThisSurveyEvent;
            ViewData["ThisSurvey"] = ThisSurvey;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];

            if (ThisSurvey.EndDate != null)
            {
                string DateDisplay = ThisSurvey.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["DateDisplay"] = DateDisplay;
            }
            else
            {
                ViewData["DateDisplay"] = "-";
            }

            int aaa = SurveyInfoEdit(ThisSurvey.Id);
            ViewData["EventSelected"] = TempData["EventSelected"];
            ViewData["SurveyActivityID"] = TempData["SurveyActivityID"];
            ViewData["EventInfo"] = TempData["EventInfo"];
            ViewData["SurveyName"] = TempData["SurveyName"];
            ViewData["SurveyID"] = TempData["SurveyID"];
            ViewData["RealtedActivity"] = TempData["RealtedActivity"];
            ViewData["RegisteredActivities"] = TempData["RegisteredActivities"];

            return View(ThisSurvey);
        }

        [HttpPost]
        public ActionResult AddChoicePES(string Complete, SurveyActivityQuestion QQQ, string Choice)
        {
            string from = (string)Session["MCFrom"];
            Guid QuestionID = (Guid)TempData["QuestionID"];
            var AllChoices = db.SurveyQuestionOption.ToList();
            var ThisOptions = AllChoices.Where(k => k.SurveyActivityQuestionID == QuestionID).ToList();
            int CountOption = ThisOptions.Count;

            if (Choice != "")
            {
                SurveyQuestionOption NewOption = new SurveyQuestionOption();
                NewOption.Order = CountOption + 1;
                NewOption.Option = Choice;
                NewOption.SurveyActivityQuestionID = QuestionID;
                db.SurveyQuestionOption.Add(NewOption);
                db.SaveChanges();
            }

            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(QuestionID);
            if (QQQ.Quesiton != null)
            {
                ThisQuestion.Quesiton = QQQ.Quesiton;
                db.Entry(ThisQuestion).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (Complete == "Complete")
            {
                var Opts = AllChoices.Where(k => k.SurveyActivityQuestionID == QuestionID).ToList();
                int Count = Opts.Count;
                if (Count <= 1)
                {
                    return RedirectToAction("EditMCPES", "Activities", new { QQQ = ThisQuestion.Id, OptionError = "OptionError" });
                }

                Guid ActivityID = (Guid)TempData["ActivityID"];
                var AllEA = db.EventActivity.ToList();
                var thisEA = AllEA.Where(k => k.ActivityID == ActivityID).ToList();

                if (from == "CreatePESDetails")
                {
                    return RedirectToAction("CreatePESDetails", "Activities", new { thissurvey = ActivityID });
                }
                else if (from == "EditSurvey")
                {
                    return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id });
                }
            }

            return RedirectToAction("EditMCPES", "Activities", new { QQQ = ThisQuestion.Id, MCFrom = from });
        }

        public ActionResult DeleteChoicePES(Guid OptionID)
        {
            Guid SurveyID = (Guid)TempData["SurveyID"];
            SurveyQuestionOption ThisOption = db.SurveyQuestionOption.Find(OptionID);
            int OptionOrder = ThisOption.Order;
            SurveyActivityQuestion ThisQuestion = db.SurveyActivityQuestion.Find(ThisOption.SurveyActivityQuestionID);

            db.SurveyQuestionOption.Remove(ThisOption);
            db.SaveChanges();

            var AllChoices = db.SurveyQuestionOption.ToList();
            var QChoices = AllChoices.Where(k => k.SurveyActivityQuestionID == ThisQuestion.Id).ToList();
            List<SurveyQuestionOption> QuestionOptions = new List<SurveyQuestionOption>();
            for (int i = 0; i < QChoices.Count; i++)
            {
                SurveyQuestionOption AddThis = db.SurveyQuestionOption.Find(QChoices[i].Id);
                QuestionOptions.Add(AddThis);
            }

            for (int j = 0; j < QuestionOptions.Count; j++)
            {
                if (QuestionOptions[j].Order > OptionOrder)
                {
                    QuestionOptions[j].Order = QuestionOptions[j].Order - 1;
                    db.Entry(QuestionOptions[j]).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("EditMCPES", "Activities", new { QQQ = ThisQuestion.Id });
        }

        public ActionResult DelFromEditPES(Guid SurveyID, Guid QuestionID)
        {
            // Question siliniyor
            SurveyActivityQuestion DeleteThis = db.SurveyActivityQuestion.Find(QuestionID);
            int missingorder = DeleteThis.Order;
            Guid QType = DeleteThis.SurveyQuestionTypeID;
            db.SurveyActivityQuestion.Remove(DeleteThis);
            db.SaveChanges();

            // Order düzenlemesi yapılıyor
            var AllQuestions = db.SurveyActivityQuestion.ToList();
            var thisquestions = AllQuestions.Where(k => k.ActivityID == SurveyID).ToList();
            for (int i = 0; i < thisquestions.Count; i++)
            {
                SurveyActivityQuestion ChangeThis = db.SurveyActivityQuestion.Find(thisquestions[i].Id);
                if (ChangeThis.Order > missingorder)
                {
                    ChangeThis.Order = ChangeThis.Order - 1;
                    db.SaveChanges();
                }
            }

            // option varsa onlar siliniyor
            var AllQuestionTypes = db.SurveyQuestionType.ToList();
            var thisQType = AllQuestionTypes.Where(l => l.Id == QType).ToList();
            if (thisQType[0].Name != "Klasik")
            {
                var AllOptions = db.SurveyQuestionOption.ToList();
                var ThisOptions = AllOptions.Where(t => t.SurveyActivityQuestionID == QuestionID).ToList();
                for (int j = 0; j < ThisOptions.Count; j++)
                {
                    SurveyQuestionOption DeleteOption = db.SurveyQuestionOption.Find(ThisOptions[j].Id);
                    db.SurveyQuestionOption.Remove(DeleteOption);
                    db.SaveChanges();
                }
            }

            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == SurveyID).ToList();

            return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id });
        }

        [HttpPost]
        public ActionResult EditSurveyInfoPES(Activity ThisSurvey)
        {
            Guid SurveyID = (Guid)TempData["SurveyID"];
            Activity EditThis = db.Activity.Find(SurveyID);
            var AllEA = db.EventActivity.ToList();
            var thisEA = AllEA.Where(k => k.ActivityID == EditThis.Id).ToList();

            string starting = ThisSurvey.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime startdate = DateTime.ParseExact(starting, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            string edate = ThisSurvey.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            string DateCompareError = "Correct";
            int result = DateTime.Compare(startdate, enddate);
            if (result >= 0)
            {
                DateCompareError = "DateCompareError";
            }

            int datecheck = 0;
            if (ThisSurvey.StartDate != null)
            {
                String currenttime = DateTime.Now.ToLongTimeString();
                String currentday = DateTime.Now.ToLongDateString();

                string[] timedivide = currenttime.Split(new[] { " " }, StringSplitOptions.None);

                string currenthour = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }

                string currentdatetime = currentday + " " + currenthour;
                DateTime CurrentTime = Convert.ToDateTime(currentdatetime);

                string day = ThisSurvey.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                string[] dayandtime = day.Split(new[] { " - " }, StringSplitOptions.None);
                string[] daydivide = dayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[0];

                string hournormal = dayandtime[1];
                ThisSurvey.StartDate = daynormal + " " + hournormal;
                DateTime sssdate = Convert.ToDateTime(ThisSurvey.StartDate);

                datecheck = DateTime.Compare(CurrentTime, sssdate);
            }

            if (ThisSurvey.EndDate != null)
            {
                String currenttime = DateTime.Now.ToLongTimeString();
                String currentday = DateTime.Now.ToLongDateString();

                string[] timedivide = currenttime.Split(new[] { " " }, StringSplitOptions.None);

                string currenthour = "";
                if (timedivide[1] == "PM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = "*" + hourdivide[0] + "*";
                    hourdivide[0] = hourdivide[0].Replace("*12*", "12").Replace("*1*", "13").Replace("*2*", "14").Replace("*3*", "15").Replace("*4*", "16").Replace("*5*", "17").Replace("*6*", "18").Replace("*7*", "19").Replace("*8*", "20").Replace("*9*", "21").Replace("*10*", "22").Replace("*11*", "23");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }
                else if (timedivide[1] == "AM")
                {
                    string[] hourdivide = timedivide[0].Split(new[] { ":" }, StringSplitOptions.None);
                    hourdivide[0] = hourdivide[0].Replace("12", "0");
                    currenthour = hourdivide[0] + ":" + hourdivide[1];
                }

                string currentdatetime = currentday + " " + currenthour;
                DateTime CurrentTime = Convert.ToDateTime(currentdatetime);

                string day = ThisSurvey.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                string[] dayandtime = day.Split(new[] { " - " }, StringSplitOptions.None);
                string[] daydivide = dayandtime[0].Split(new[] { "/" }, StringSplitOptions.None);
                string daynormal = daydivide[2] + "-" + daydivide[1] + "-" + daydivide[0];

                string hournormal = dayandtime[1];
                ThisSurvey.EndDate = daynormal + " " + hournormal;
            }

            if (datecheck >= 0 && DateCompareError == "DateCompareError")
            {
                ViewData["DateError"] = "DateError";
                return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id, DateError = "DateError", DateCompareError = "DateCompareError" });
            }
            else if (datecheck < 0 && DateCompareError == "DateCompareError")
            {
                return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id, DateCompareError = "DateCompareError" });
            }
            else if (datecheck >= 0 && DateCompareError != "DateCompareError")
            {
                return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id, DateError = "DateError" });
            }
            else
            {
                DateTime sdate = Convert.ToDateTime(ThisSurvey.StartDate);
                EditThis.StartDateTime = sdate;

                string changedate = ThisSurvey.StartDate;
                string[] divide = changedate.Split(new[] { "-" }, StringSplitOptions.None);
                string[] divide2 = divide[2].Split(new[] { " " }, StringSplitOptions.None);
                string month1 = "*" + divide[1] + "*";
                string month = month1.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
                EditThis.StartDate = divide2[0] + " " + month + " " + divide[0] + " - " + divide2[1];

                DateTime Edate = Convert.ToDateTime(ThisSurvey.EndDate);
                EditThis.EndDateTime = Edate;

                string changedateE = ThisSurvey.EndDate;
                string[] divideE = changedateE.Split(new[] { "-" }, StringSplitOptions.None);
                string[] divide2E = divideE[2].Split(new[] { " " }, StringSplitOptions.None);
                string month1E = "*" + divideE[1] + "*";
                string monthE = month1E.Replace("*01*", "January").Replace("*02*", "February").Replace("*03*", "March").Replace("*04*", "April").Replace("*05*", "May").Replace("*06*", "June").Replace("*07*", "July").Replace("*08*", "August").Replace("*09*", "September").Replace("*10*", "October").Replace("*11*", "November").Replace("*12*", "December");
                EditThis.EndDate = divide2E[0] + " " + monthE + " " + divideE[0] + " - " + divide2E[1];

                db.Entry(EditThis).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("EditPES", "Activities", new { SurveyEventID = thisEA[0].Id });
        }

        public ActionResult CompleteSurveyPES(Guid SurveyEventID)
        {
            EventActivity ThisEA = db.EventActivity.Find(SurveyEventID);
            ThisEA.IsActive = true;
            db.Entry(ThisEA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PESurveyOrg", "Activities");
        }

        public ActionResult CancelSurveyPES(Guid SurveyEventID)
        {
            EventActivity ThisEA = db.EventActivity.Find(SurveyEventID);
            ThisEA.IsActive = false;
            db.Entry(ThisEA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PESurveyOrg", "Activities");
        }

        public ActionResult ChatDiscussionList()
        {
            return View();
        }

        public ActionResult Discussion()
        {
            return View();
        }

        public ActionResult SurveyList()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllEventActivities = db.EventActivity.ToList();
            var ThisEventActivities = AllEventActivities.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            var AllActivityCategories = db.ActivityCategory.ToList();
            var ThisActivityCategory = AllActivityCategories.Where(t => t.Name == "Anket").ToList();

            List<Activity> ThisSurveys = new List<Activity>();
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                Activity AddThis = db.Activity.Find(ThisEventActivities[i].ActivityID);

                if (AddThis.ActivityCategoryID == ThisActivityCategory[0].Id && AddThis.IsPostEventSurvey == false)
                {
                    if (AddThis.EndDate == null)
                    {
                        AddThis.EndDate = "31 December 2030 - 00:00";
                    }
                    double unixTime = SortingActivityTime(AddThis.EndDate);
                    AddThis.StartDate = unixTime.ToString();
                    ThisSurveys.Add(AddThis);
                }
            }

            List<Activity> SortedSurveyList = new List<Activity>();
            SortedSurveyList = ThisSurveys.OrderBy(d => Convert.ToDouble(d.StartDate)).ToList();

            List<EventActivity> ThisEventSurvey = new List<EventActivity>();
            for (int j = 0; j < SortedSurveyList.Count; j++)
            {
                var AddEA = ThisEventActivities.Where(l => l.ActivityID == SortedSurveyList[j].Id).ToList();
                EventActivity EA = db.EventActivity.Find(AddEA[0].Id);
                ThisEventSurvey.Add(EA);
            }

            ViewData["SurveyList"] = SortedSurveyList;
            ViewData["EventSurveyList"] = ThisEventSurvey;
            return View();
        }

        public ActionResult AttendSurvey(Guid SurveyEventID)
        {
            EventActivity ThisSurveyEvent = db.EventActivity.Find(SurveyEventID);
            Activity ThisSurvey = db.Activity.Find(ThisSurveyEvent.ActivityID);

            if (ThisSurveyEvent.IsActive == true)
            {
                ViewData["IsActive"] = "true";
            }
            else
            {
                ViewData["IsActive"] = "false";
            }

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            if (ThisSurvey.EventOrEventActivityForSurveyAndDiscussion == IDofEvent[0].Id)
            {
                ViewData["RelatedActivity"] = IDofEvent[0].Name;
            }
            else
            {
                Activity Related = db.Activity.Find(ThisSurvey.EventOrEventActivityForSurveyAndDiscussion);
                ViewData["RelatedActivity"] = Related.Name;
            }

            ViewData["ThisSurveyEvent"] = ThisSurveyEvent;
            ViewData["ThisSurvey"] = ThisSurvey;

            ControllerToSurvey(ThisSurvey.Id);
            ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
            ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
            ViewData["QuestionOptions"] = TempData["QuestionOptions"];
            ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];

            if (ThisSurvey.EndDate != null)
            {
                string DateDisplay = ThisSurvey.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["DateDisplay"] = DateDisplay;
            }
            else
            {
                ViewData["DateDisplay"] = "-";
            }

            return View();
        }

        public ActionResult PES()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            var TypeOfActivity = db.ActivityCategory.ToList();
            var SurveyType = TypeOfActivity.Where(k => k.Name == "Anket").ToList();

            // PES oluşturulmuş mu oluşturulmamış mı diye bakıyor
            Activity PES = null;
            for (int i = 0; i < ThisEA.Count; i++)
            {
                Activity AddThis = db.Activity.Find(ThisEA[i].ActivityID);
                if (AddThis.ActivityCategoryID == SurveyType[0].Id && AddThis.IsPostEventSurvey == true)
                {
                    PES = AddThis;
                    ViewData["SurveyEventID"] = ThisEA[i].Id;
                }
            }

            // PES oluşturulmuş ise sorularını falan çekiyor
            if (PES != null)
            {
                string EndDate = PES.EndDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["EndDate"] = EndDate;
                string StartDate = PES.StartDate.Replace("January", "Ocak").Replace("February", "Şubat").Replace("March", "Mart").Replace("April", "Nisan").Replace("May", "Mayıs").Replace("June", "Haziran").Replace("July", "Temmuz").Replace("August", "Ağustos").Replace("September", "Eylül").Replace("October", "Ekim").Replace("November", "Kasım").Replace("December", "Aralık");
                ViewData["StartDate"] = StartDate;

                ControllerToSurvey(PES.Id);
                var PESEvent = ThisEA.Where(l => l.ActivityID == PES.Id).ToList();
                if (PESEvent[0].IsActive == true)
                {
                    ViewData["IsActive"] = "true";
                }
                else
                {
                    ViewData["IsActive"] = "false";
                }
                ViewData["EventName"] = IDofEvent[0].Name;
                ViewData["PES"] = PES;
                ViewData["SortedSurveyQuestions"] = TempData["SortedSurveyQuestions"];
                ViewData["OptionNumberOfQuestions"] = TempData["OptionNumberOfQuestions"];
                ViewData["QuestionOptions"] = TempData["QuestionOptions"];
                ViewData["TypeOfQuestionByOrder"] = TempData["TypeOfQuestionByOrder"];
            }
            else
            {
                ViewData["EventName"] = IDofEvent[0].Name;
                ViewData["IsActive"] = "";
                ViewData["PES"] = null;
                ViewData["SortedSurveyQuestions"] = null;
                ViewData["OptionNumberOfQuestions"] = null;
                ViewData["QuestionOptions"] = null;
                ViewData["TypeOfQuestionByOrder"] = null;
                ViewData["StartDate"] = "";
                ViewData["EndDate"] = "";
                ViewData["SurveyEventID"] = Guid.Empty;
            }

            return View();
        }

        public ActionResult NewCoffee()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllMaps = db.MapEventActivity.ToList();
            var ThisMaps = AllMaps.Where(k => k.EventOrEventActivityID == IDofEvent[0].Id).ToList();
            List<Map> MapList = new List<Map>();
            for (int i = 0; i < ThisMaps.Count; i++)
            {
                Map AddThis = db.Map.Find(ThisMaps[i].MapID);
                MapList.Add(AddThis);
            }

            ViewData["MapList"] = MapList;

            var alltypes = db.UserType.ToList();
            var attendertype = alltypes.Where(k => k.Name == "Acente").ToList();
            var allusers = db.User.ToList();
            var organizationlist = allusers.Where(k => k.UserTypeID == attendertype[0].Id).ToList();
            List<User> Organization = new List<User>();
            for (int i = 0; i < organizationlist.Count; i++)
            {
                User AddThis = db.User.Find(organizationlist[i].Id);
                Organization.Add(AddThis);
            }
            ViewData["OrgList"] = Organization;

            return View();
        }

        [HttpPost]
        public ActionResult NewCoffee(Activity act, Guid Places, Guid Responsible)
        {
            string DateCompareError = "Correct";
            string SDateError = "Correct";
            string EDateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";
            string PlaceError = "PlaceError";
            string OrgError = "OrgError";
            bool Error = false;

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllMaps = db.MapEventActivity.ToList();
            var ThisMaps = AllMaps.Where(k => k.EventOrEventActivityID == IDofEvent[0].Id).ToList();
            List<Map> MapList = new List<Map>();
            for (int i = 0; i < ThisMaps.Count; i++)
            {
                Map AddThis = db.Map.Find(ThisMaps[i].MapID);
                MapList.Add(AddThis);
            }

            ViewData["MapList"] = MapList;

            var alltypes = db.UserType.ToList();
            var attendertype = alltypes.Where(k => k.Name == "Acente").ToList();
            var allusers = db.User.ToList();
            var organizationlist = allusers.Where(k => k.UserTypeID == attendertype[0].Id).ToList();
            List<User> Organization = new List<User>();
            for (int i = 0; i < organizationlist.Count; i++)
            {
                User AddThis = db.User.Find(organizationlist[i].Id);
                Organization.Add(AddThis);
            }
            ViewData["OrgList"] = Organization;

            if ((act.StartDate != null && act.EndDate != null))
            {
                string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                ViewData["result"] = result;
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                    Error = true;
                }
            }
            else if (act.StartDate == null && act.EndDate != null)
            {
                SDateError = "SDateError";
                Error = true;
            }
            else if (act.StartDate != null && act.EndDate == null)
            {
                EDateError = "EDateError";
                Error = true;
            }
            else
            {
                SDateError = "SDateError";
                EDateError = "EDateError";
                Error = true;
            }
            if (act.Name == null)
            {
                NameError = "NameError";
                Error = true;
            }
            if (Places.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                PlaceError = "PlaceError";
                Error = true;
            }
            if (Responsible.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                OrgError = "OrgError";
                Error = true;
            }
            if (act.Summary == null)
            {
                ContentError = "ContentError";
                Error = true;
            }
            if (Error == true)
            {
                ViewData["DateCompareError"] = DateCompareError;
                ViewData["SDateError"] = SDateError;
                ViewData["EDateError"] = EDateError;
                ViewData["NameError"] = NameError;
                ViewData["ContentError"] = ContentError;
                ViewData["PlaceError"] = PlaceError;
                ViewData["OrgError"] = OrgError;
                return View(act);
            }

            var AllActivityCategories = db.ActivityCategory.ToList();
            var ThisCategory = AllActivityCategories.Where(k => k.Name == "Kahve Molası-Yemek-Parti").ToList();

            string sdate2 = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime startdate2 = DateTime.ParseExact(sdate2, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            string edate2 = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime enddate2 = DateTime.ParseExact(edate2, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            act.StartDateTime = startdate2;
            act.EndDateTime = enddate2;
            act.ActivityCategoryID = ThisCategory[0].Id;
            act.IsPostEventSurvey = false;
            act.IsActive = true;

            db.Activity.Add(act);
            db.SaveChanges();

            using (EventAppContext dbUE = new EventAppContext())
            {

                EventActivity EActivity = new EventActivity();
                EActivity.ActivityID = act.Id;
                EActivity.EventID = IDofEvent[0].Id;
                dbUE.EventActivity.Add(EActivity);
                dbUE.SaveChanges();

                using (EventAppContext dbAgenda = new EventAppContext())
                {
                    // AgendaEventActivity kaydediliyor
                    AgendaEventActivity agenda = new AgendaEventActivity();
                    agenda.EventActivityID = EActivity.Id;
                    dbAgenda.AgendaEventActivity.Add(agenda);
                    dbAgenda.SaveChanges();
                }

                using (EventAppContext dbOrg = new EventAppContext())
                {
                    // AddedPlaces Kaydediliyor
                    ResponsibleEventActivity org = new ResponsibleEventActivity();
                    org.EventActivityID = EActivity.Id;
                    org.ResponsibleID = Responsible;
                    dbOrg.ResponsibleEventActivity.Add(org);
                    dbOrg.SaveChanges();
                }

                using (EventAppContext dbPL = new EventAppContext())
                {
                    // AddedPlaces Kaydediliyor
                    MapEventActivity mea = new MapEventActivity();
                    mea.EventOrEventActivityID = EActivity.Id;
                    mea.MapID = Places;
                    dbPL.MapEventActivity.Add(mea);
                    dbPL.SaveChanges();
                }
            }

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult CoffeeEdit(Guid? IDofActivity)
        {
            Activity ThisActivity = db.Activity.Find(IDofActivity);

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllMaps = db.MapEventActivity.ToList();
            var ThisMaps = AllMaps.Where(k => k.EventOrEventActivityID == IDofEvent[0].Id).ToList();
            List<Map> MapList = new List<Map>();
            for (int i = 0; i < ThisMaps.Count; i++)
            {
                Map AddThis = db.Map.Find(ThisMaps[i].MapID);
                MapList.Add(AddThis);
            }
            ViewData["MapList"] = MapList;

            var alltypes = db.UserType.ToList();
            var attendertype = alltypes.Where(k => k.Name == "Acente").ToList();
            var allusers = db.User.ToList();
            var organizationlist = allusers.Where(k => k.UserTypeID == attendertype[0].Id).ToList();
            List<User> Organization = new List<User>();
            for (int i = 0; i < organizationlist.Count; i++)
            {
                User AddThis = db.User.Find(organizationlist[i].Id);
                Organization.Add(AddThis);
            }
            ViewData["OrgList"] = Organization;

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.ActivityID == ThisActivity.Id).ToList();

            var AllMapEvent = db.MapEventActivity.ToList();
            var TheMap = AllMapEvent.Where(k => k.EventOrEventActivityID == ThisEA[0].Id).ToList();
            Map ThisMap = db.Map.Find(TheMap[0].MapID);
            ViewData["ThisMap"] = ThisMap;
            var allResp = db.ResponsibleEventActivity.ToList();
            var TheResp = allResp.Where(k => k.EventActivityID == ThisEA[0].Id).ToList();
            User ThisResp = db.User.Find(TheResp[0].ResponsibleID);
            ViewData["ThisOrg"] = ThisResp;
            ViewData["ThisCoffee"] = ThisActivity;

            return View(ThisActivity);
        }

        [HttpPost]
        public ActionResult CoffeeEdit(Activity act, Guid Places, Guid Responsible)
        {
            string DateCompareError = "Correct";
            string SDateError = "Correct";
            string EDateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";
            string PlaceError = "Correct";
            string OrgError = "Correct";
            bool Error = false;

            Guid IDofActivity = (Guid)TempData["IDofActivity"];
            var AllEA2 = db.EventActivity.ToList();
            var ThisEA2 = AllEA2.Where(k => k.ActivityID == IDofActivity).ToList();

            var AllMapEvent2 = db.MapEventActivity.ToList();
            var TheMap = AllMapEvent2.Where(k => k.EventOrEventActivityID == ThisEA2[0].Id).ToList();
            Map ThisMap = db.Map.Find(TheMap[0].MapID);
            ViewData["ThisMap"] = ThisMap;

            var allResp = db.ResponsibleEventActivity.ToList();
            var TheResp = allResp.Where(k => k.EventActivityID == ThisEA2[0].Id).ToList();
            User ThisResp = db.User.Find(TheResp[0].ResponsibleID);
            ViewData["ThisOrg"] = ThisResp;

            Activity ThisA = db.Activity.Find(IDofActivity);
            ViewData["ThisCoffee"] = ThisA;

            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllMaps = db.MapEventActivity.ToList();
            var ThisMaps = AllMaps.Where(k => k.EventOrEventActivityID == IDofEvent[0].Id).ToList();
            List<Map> MapList = new List<Map>();
            for (int i = 0; i < ThisMaps.Count; i++)
            {
                Map AddThis = db.Map.Find(ThisMaps[i].MapID);
                MapList.Add(AddThis);
            }

            ViewData["MapList"] = MapList;

            var alltypes = db.UserType.ToList();
            var attendertype = alltypes.Where(k => k.Name == "Acente").ToList();
            var allusers = db.User.ToList();
            var organizationlist = allusers.Where(k => k.UserTypeID == attendertype[0].Id).ToList();
            List<User> Organization = new List<User>();
            for (int i = 0; i < organizationlist.Count; i++)
            {
                User AddThis = db.User.Find(organizationlist[i].Id);
                Organization.Add(AddThis);
            }
            ViewData["OrgList"] = Organization;

            if ((act.StartDate != null && act.EndDate != null))
            {
                string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                ViewData["result"] = result;
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                    Error = true;
                }
            }
            else if (act.StartDate == null && act.EndDate != null)
            {
                SDateError = "SDateError";
                Error = true;
            }
            else if (act.StartDate != null && act.EndDate == null)
            {
                EDateError = "EDateError";
                Error = true;
            }
            else
            {
                SDateError = "SDateError";
                EDateError = "EDateError";
                Error = true;
            }
            if (act.Name == null)
            {
                NameError = "NameError";
                Error = true;
            }
            if (Places.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                PlaceError = "PlaceError";
                Error = true;
            }
            if (Responsible.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                OrgError = "OrgError";
                Error = true;
            }
            if (act.Summary == null)
            {
                ContentError = "ContentError";
                Error = true;
            }
            if (Error == true)
            {
                ViewData["DateCompareError"] = DateCompareError;
                ViewData["SDateError"] = SDateError;
                ViewData["EDateError"] = EDateError;
                ViewData["NameError"] = NameError;
                ViewData["ContentError"] = ContentError;
                ViewData["PlaceError"] = PlaceError;
                ViewData["OrgError"] = OrgError;
                return View(act);
            }

            var AllActivityCategories = db.ActivityCategory.ToList();
            var ThisCategory = AllActivityCategories.Where(k => k.Name == "Kahve Molası-Yemek-Parti").ToList();

            string sdate2 = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime startdate2 = DateTime.ParseExact(sdate2, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            string edate2 = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
            DateTime enddate2 = DateTime.ParseExact(edate2, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

            Activity activity = db.Activity.Find(IDofActivity);
            activity.StartDate = act.StartDate;
            activity.EndDate = act.EndDate;
            activity.StartDateTime = startdate2;
            activity.EndDateTime = enddate2;
            activity.Name = act.Name;
            activity.Summary = act.Summary;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            var AllEA = db.EventActivity.ToList();
            var ThisEA = AllEA.Where(k => k.ActivityID == activity.Id).ToList();

            var AllMapEvent = db.MapEventActivity.ToList();
            var ThisMapEvent = AllMapEvent.Where(k => k.EventOrEventActivityID == ThisEA[0].Id).ToList();

            MapEventActivity Change = db.MapEventActivity.Find(ThisMapEvent[0].Id);
            Change.MapID = Places;
            db.Entry(Change).State = EntityState.Modified;
            db.SaveChanges();

            var AllRespEvent = db.ResponsibleEventActivity.ToList();
            var ThisRespEvent = AllRespEvent.Where(k => k.EventActivityID == ThisEA[0].Id).ToList();

            ResponsibleEventActivity ChangeThis = db.ResponsibleEventActivity.Find(ThisRespEvent[0].Id);
            ChangeThis.ResponsibleID = Responsible;
            db.Entry(ChangeThis).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        //Panel
        public ActionResult CreatePanel(Activity act, string edit, Guid? DeleteAfterEdit, Guid? Panelist, string CompletePanel, Guid? Speakers, Guid? DeleteSpeaker, Guid? Responsibles, Guid? DeleteResponsible, Guid? Places, Guid? DeletePlace, string IsNew, List<User> AddedSpeakers, List<User> AddedResponsibles, List<Map> AddedPlaces)
        {
            if (Panelist != null)
            {
                ViewData["Panelist"] = Panelist;
            }
            else
            {
                ViewData["Panelist"] = Guid.Empty;
            }
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

            List<User> PanelistList = new List<User>();
            var TypeOfUser = db.UserType.ToList();
            var AttenderTypeID = TypeOfUser.Where(k => k.Name == "Konuşmacı").ToList();
            for (int i = 0; i < UserOfEvent.Count; i++)
            {
                if (UserOfEvent[i].UserTypeID == AttenderTypeID[0].Id)
                {
                    User newuser = db.User.Find(UserOfEvent[i].Id);
                    PanelistList.Add(newuser);
                }
            }
            ViewData["PanelistList"] = PanelistList;

            Session["PanelBackButton"] = Session["PanelBackButton"];
            if (DeleteAfterEdit != null)
            {
                Session["DeletAfterEdit"] = DeleteAfterEdit;
            }
            else
            {
                Session["DeletAfterEdit"] = Session["DeletAfterEdit"];
            }

            if (edit == "edit" && IsNew != "newpanel")
            {
                AddedSpeakers = (List<User>)TempData["spkrs"];
                Session["PanelAddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = (List<User>)TempData["rspnbls"];
                Session["PanelAddedResponsibles"] = AddedResponsibles;
                AddedPlaces = (List<Map>)TempData["plcs"];
                Session["PanelAddedPlaces"] = AddedPlaces;
                act = (Activity)TempData["act"];
            }
            else if (IsNew == "newpanel")
            {
                AddedSpeakers = new List<User>();
                Session["PanelAddedSpeakers"] = AddedSpeakers;
                AddedResponsibles = new List<User>();
                Session["PanelAddedResponsibles"] = AddedResponsibles;
                AddedPlaces = new List<Map>();
                Session["PanelAddedPlaces"] = AddedPlaces;
            }
            else
            {
                AddedSpeakers = (List<User>)Session["PanelAddedSpeakers"];
                AddedResponsibles = (List<User>)Session["PanelAddedResponsibles"];
                AddedPlaces = (List<Map>)Session["PanelAddedPlaces"];
            }

            string CancelError = "Correct";
            if (DeleteSpeaker != null || DeleteResponsible != null || DeletePlace != null)
            {
                CancelError = "Error";
            }

            if (Speakers.ToString() != "00000000-0000-0000-0000-000000000000" && Speakers != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedSpeakers.Count; i++)
                {
                    if (AddedSpeakers[i].Id == Speakers)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedSpeakers.Count == 0)
                {
                    User newspeaker = db.User.Find(Speakers);
                    AddedSpeakers.Add(newspeaker);
                    Session["PanelAddedSpeakers"] = AddedSpeakers;
                }
            }

            if (DeleteSpeaker.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteSpeaker != null)
            {
                User Sdel = db.User.Find(DeleteSpeaker);
                int count = AddedSpeakers.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedSpeakers[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "delete";
                    }
                }
                if (delete == "delete")
                {
                    AddedSpeakers.RemoveAt(order);
                    Session["PanelAddedSpeakers"] = AddedSpeakers;
                }
            }

            if (Responsibles.ToString() != "00000000-0000-0000-0000-000000000000" && Responsibles != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedResponsibles.Count; i++)
                {
                    if (AddedResponsibles[i].Id == Responsibles)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedResponsibles.Count == 0)
                {
                    User newresponsible = db.User.Find(Responsibles);
                    AddedResponsibles.Add(newresponsible);
                    Session["PanelAddedResponsibles"] = AddedResponsibles;
                }
            }

            if (DeleteResponsible.ToString() != "00000000-0000-0000-0000-000000000000" && DeleteResponsible != null)
            {
                User Sdel = db.User.Find(DeleteResponsible);
                int count = AddedResponsibles.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedResponsibles[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedResponsibles.RemoveAt(order);
                    Session["PanelAddedResponsibles"] = AddedResponsibles;
                }
            }

            if (Places.ToString() != "00000000-0000-0000-0000-000000000000" && Places != null && CancelError == "Correct")
            {
                int IsAdded = 0;
                for (int i = 0; i < AddedPlaces.Count; i++)
                {
                    if (AddedPlaces[i].Id == Places)
                    {
                        IsAdded = 1;
                    }
                }
                if (IsAdded != 1 || AddedPlaces.Count == 0)
                {
                    Map newplace = db.Map.Find(Places);
                    AddedPlaces.Add(newplace);
                    Session["PanelAddedPlaces"] = AddedPlaces;
                }
            }

            if (DeletePlace.ToString() != "00000000-0000-0000-0000-000000000000" && DeletePlace != null)
            {
                Map Sdel = db.Map.Find(DeletePlace);
                int count = AddedPlaces.Count();
                int order = 0;
                String delete = "Empty";
                for (int i = 0; i < count; i++)
                {
                    if (AddedPlaces[i].Id == Sdel.Id)
                    {
                        order = i;
                        i--;
                        count--;
                        delete = "Delete";
                    }
                }
                if (delete == "Delete")
                {
                    AddedPlaces.RemoveAt(order);
                    Session["PanelAddedPlaces"] = AddedPlaces;
                }
            }

            ViewData["PanelAddedSpeakers"] = AddedSpeakers;
            ViewData["PanelAddedResponsibles"] = AddedResponsibles;
            ViewData["PanelAddedPlaces"] = AddedPlaces;

            List<User> SpeakerList = SpeakerListOfEvent(AddedSpeakers);
            List<User> ResponsibleList = ResponsibleListOfEvent(AddedResponsibles);
            List<Map> MapList = MapListOfEvent(AddedPlaces);
            ViewData["PanelSpeakerList"] = SpeakerList;
            ViewData["PanelResponsibleList"] = ResponsibleList;
            ViewData["PanelMapList"] = MapList;

            if (ModelState.IsValid)
            {
                List<string> ValidationErrors = PanelValidation(act);
                if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompletePanel == "CompletePanel" && Panelist.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    //Burada Create Edicez
                    TempData["AddedSpeaker"] = AddedSpeakers;
                    TempData["AddedResponsible"] = AddedResponsibles;
                    TempData["AddedPlaces"] = AddedPlaces;
                    TempData["Model"] = act;
                    TempData["PanelistID"] = Panelist;
                    Session["DeletAfterEdit"] = Session["DeletAfterEdit"];
                    return RedirectToAction("PanelSubmit", "Activities");
                }
                else if (ValidationErrors[0] == "Correct" && ValidationErrors[1] == "Correct" && ValidationErrors[2] == "Correct" && ValidationErrors[3] == "Correct" && ValidationErrors[4] == "Correct" && ValidationErrors[5] == "Correct" && CompletePanel != "CompletePanel" && edit != "edit")
                {
                    if (Panelist.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        ViewData["PanelistError"] = "PanelistError";
                    }
                    ViewData["PanelValidationErrors"] = ValidationErrors;
                    return View();
                }
                else if (edit == "edit")
                {
                    if (Panelist.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        ViewData["PanelistError"] = "PanelistError";
                    }
                    ViewData["PanelValidationErrors"] = ValidationErrors;
                    return View(act);
                }
                else
                {
                    if (Panelist.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        ViewData["PanelistError"] = "PanelistError";
                    }
                    ViewData["PanelValidationErrors"] = ValidationErrors;
                    return View();
                }
            }
            else
            {
                if (Panelist.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    ViewData["PanelistError"] = "PanelistError";
                }
                string correct = "Correct";
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ValidationErrors.Add(correct);
                ViewData["PanelValidationErrors"] = ValidationErrors;

                return View();
            }
        }

        public dynamic PanelValidation(Activity act)
        {
            string DateCompareError = "Correct";
            string SDateError = "Correct";
            string EDateError = "Correct";
            string NameError = "Correct";
            string ContentError = "Correct";
            string SummaryError = "Correct";


            if ((act.StartDate != null && act.EndDate != null))
            {
                string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                int result = DateTime.Compare(startdate, enddate);
                ViewData["result"] = result;
                if (result >= 0)
                {
                    DateCompareError = "DateCompareError";
                }
            }
            else if (act.StartDate == null && act.EndDate != null)
            {
                SDateError = "SDateError";
            }
            else if (act.StartDate != null && act.EndDate == null)
            {
                EDateError = "EDateError";
            }
            else
            {
                SDateError = "SDateError";
                EDateError = "EDateError";
            }

            if (act.Name == null)
            {
                NameError = "NameError";
            }

            if (act.Content == null)
            {
                ContentError = "ContentError";
            }

            if (act.Summary == null)
            {
                SummaryError = "SummaryError";
            }

            List<string> ValidationErrors = new List<string>();
            ValidationErrors.Add(DateCompareError);
            ValidationErrors.Add(SDateError);
            ValidationErrors.Add(EDateError);
            ValidationErrors.Add(NameError);
            ValidationErrors.Add(SummaryError);
            ValidationErrors.Add(ContentError);

            return ValidationErrors;
        }

        public ActionResult PanelSubmit()
        {
            Activity act = (Activity)TempData["Model"];
            List<User> AddedSpeakers = (List<User>)TempData["AddedSpeaker"];
            List<User> AddedResponsibles = (List<User>)TempData["AddedResponsible"];
            List<Map> AddedPlaces = (List<Map>)TempData["AddedPlaces"];
            Guid PanelistID = (Guid)TempData["PanelistID"];

            if (ModelState.IsValid)
            {
                Guid deletePrevious = (Guid)Session["DeletAfterEdit"];
                if (deletePrevious.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    using (EventAppContext db = new EventAppContext())
                    {
                        // Activity Kaydediliyor
                        List<ActivityCategory> Category = db.ActivityCategory.ToList();
                        Guid CategoryID = Guid.Empty;
                        for (int i = 0; i < Category.Count; i++)
                        {
                            if (Category[i].Name == "Panel")
                            {
                                CategoryID = Category[i].Id;
                            }
                        }

                        string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                        DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                        act.StartDateTime = startdate;
                        act.EndDateTime = enddate;
                        act.ActivityCategoryID = CategoryID;
                        act.IsPostEventSurvey = false;
                        act.IsActive = true;
                        act.EventOrEventActivityForSurveyAndDiscussion = PanelistID;

                        db.Activity.Add(act);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            //Activity ID'si oluştuktan sonra EventActivity'e kaydediliyor
                            User LoggedUser = (User)Session["LoggedIn"];
                            var fromAllEvents = db.Event.ToList();
                            var IDofEvent = fromAllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

                            EventActivity EActivity = new EventActivity();
                            EActivity.ActivityID = act.Id;
                            EActivity.EventID = IDofEvent[0].Id;
                            dbUE.EventActivity.Add(EActivity);
                            dbUE.SaveChanges();

                            using (EventAppContext dbAgenda = new EventAppContext())
                            {
                                // AgendaEventActivity kaydediliyor
                                AgendaEventActivity agenda = new AgendaEventActivity();
                                agenda.EventActivityID = EActivity.Id;
                                dbAgenda.AgendaEventActivity.Add(agenda);
                                dbAgenda.SaveChanges();
                            }

                            using (EventAppContext dbSP = new EventAppContext())
                            {
                                // AddedSpeakers kaydediliyor
                                for (int i = 0; i < AddedSpeakers.Count; i++)
                                {
                                    SpeakerEventActivity sea = new SpeakerEventActivity();
                                    sea.EventActivityID = EActivity.Id;
                                    sea.SpeakerID = AddedSpeakers[i].Id;
                                    dbSP.SpeakerEventActivity.Add(sea);
                                    dbSP.SaveChanges();
                                }
                            }

                            using (EventAppContext dbRS = new EventAppContext())
                            {
                                // AddedResponsibles kaydediliyor
                                for (int i = 0; i < AddedResponsibles.Count; i++)
                                {
                                    ResponsibleEventActivity rea = new ResponsibleEventActivity();
                                    rea.EventActivityID = EActivity.Id;
                                    rea.ResponsibleID = AddedResponsibles[i].Id;
                                    dbRS.ResponsibleEventActivity.Add(rea);
                                    dbRS.SaveChanges();
                                }
                            }

                            using (EventAppContext dbPL = new EventAppContext())
                            {
                                // AddedPlaces Kaydediliyor
                                for (int i = 0; i < AddedPlaces.Count; i++)
                                {
                                    MapEventActivity mea = new MapEventActivity();
                                    mea.EventOrEventActivityID = EActivity.Id;
                                    mea.MapID = AddedPlaces[i].Id;
                                    dbPL.MapEventActivity.Add(mea);
                                    dbPL.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    Activity ChangeThisActivity = db.Activity.Find(deletePrevious);
                    // Yeni Activity oluştur
                    List<ActivityCategory> Category = db.ActivityCategory.ToList();
                    Guid CategoryID = Guid.Empty;
                    for (int i = 0; i < Category.Count; i++)
                    {
                        if (Category[i].Name == "Panel")
                        {
                            CategoryID = Category[i].Id;
                        }
                    }

                    string sdate = act.StartDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                    DateTime startdate = DateTime.ParseExact(sdate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                    string edate = act.EndDate.Replace(" January ", "/01/").Replace(" February ", "/02/").Replace(" March ", "/03/").Replace(" April ", "/04/").Replace(" May ", "/05/").Replace(" June ", "/06/").Replace(" July ", "/07/").Replace(" August ", "/08/").Replace(" September ", "/09/").Replace(" October ", "/10/").Replace(" November ", "/11/").Replace(" December ", "/12/");
                    DateTime enddate = DateTime.ParseExact(edate, "dd/MM/yyyy - HH:mm", new CultureInfo("tr"));

                    act.StartDateTime = startdate;
                    act.EndDateTime = enddate;
                    act.ActivityCategoryID = CategoryID;
                    act.IsPostEventSurvey = false;
                    act.IsActive = true;
                    act.EventOrEventActivityForSurveyAndDiscussion = PanelistID;

                    db.Activity.Add(act);
                    db.SaveChanges();

                    // Agenda-Speaker-Responsible-Map'leri sil
                    var AllEventActivities = db.EventActivity.ToList();
                    var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == deletePrevious).ToList();
                    Guid ThisEventActivityID = EditEventActivityID[0].Id;
                    EventActivity ThisEventActivity = db.EventActivity.Find(ThisEventActivityID);

                    var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                    var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < SpeakerList.Count; i++)
                    {
                        SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                        db.SpeakerEventActivity.Remove(deleteSpeaker);
                        db.SaveChanges();
                    }

                    var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                    var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < ResponsibleList.Count; i++)
                    {
                        ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                        db.ResponsibleEventActivity.Remove(deleteResponsible);
                        db.SaveChanges();
                    }

                    var AllAddedPlaceList = db.MapEventActivity.ToList();
                    var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < PlaceList.Count; i++)
                    {
                        MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                        db.MapEventActivity.Remove(deleteMap);
                        db.SaveChanges();
                    }

                    var AllAddedAgenda = db.AgendaEventActivity.ToList();
                    var AgendaList = AllAddedAgenda.Where(t => t.EventActivityID == ThisEventActivityID).ToList();
                    for (int i = 0; i < AgendaList.Count; i++)
                    {
                        AgendaEventActivity deleteagenda = db.AgendaEventActivity.Find(AgendaList[i].Id);
                        db.AgendaEventActivity.Remove(deleteagenda);
                        db.SaveChanges();
                    }

                    // EventActivity'deki Activity ID'yi sil
                    ThisEventActivity.ActivityID = act.Id;
                    db.SaveChanges();

                    // Eski Activity siliniyor
                    db.Activity.Remove(ChangeThisActivity);
                    db.SaveChanges();

                    // Agenda-Speaker-Responsible-Map'i oluştur
                    using (EventAppContext dbAgenda = new EventAppContext())
                    {
                        // AgendaEventActivity kaydediliyor
                        AgendaEventActivity agenda = new AgendaEventActivity();
                        agenda.EventActivityID = ThisEventActivity.Id;
                        dbAgenda.AgendaEventActivity.Add(agenda);
                        dbAgenda.SaveChanges();
                    }

                    using (EventAppContext dbSP = new EventAppContext())
                    {
                        // AddedSpeakers kaydediliyor
                        for (int i = 0; i < AddedSpeakers.Count; i++)
                        {
                            SpeakerEventActivity sea = new SpeakerEventActivity();
                            sea.EventActivityID = ThisEventActivity.Id;
                            sea.SpeakerID = AddedSpeakers[i].Id;
                            dbSP.SpeakerEventActivity.Add(sea);
                            dbSP.SaveChanges();
                        }
                    }

                    using (EventAppContext dbRS = new EventAppContext())
                    {
                        // AddedResponsibles kaydediliyor
                        for (int i = 0; i < AddedResponsibles.Count; i++)
                        {
                            ResponsibleEventActivity rea = new ResponsibleEventActivity();
                            rea.EventActivityID = ThisEventActivity.Id;
                            rea.ResponsibleID = AddedResponsibles[i].Id;
                            dbRS.ResponsibleEventActivity.Add(rea);
                            dbRS.SaveChanges();
                        }
                    }

                    using (EventAppContext dbPL = new EventAppContext())
                    {
                        // AddedPlaces Kaydediliyor
                        for (int i = 0; i < AddedPlaces.Count; i++)
                        {
                            MapEventActivity mea = new MapEventActivity();
                            mea.EventOrEventActivityID = ThisEventActivity.Id;
                            mea.MapID = AddedPlaces[i].Id;
                            dbPL.MapEventActivity.Add(mea);
                            dbPL.SaveChanges();
                        }
                    }
                }

                TempData["AddedSpeaker"] = AddedSpeakers;
                TempData["AddedResponsible"] = AddedResponsibles;
                TempData["AddedPlaces"] = AddedPlaces;
                TempData["Model"] = act;

                int countRespAct = RespAct();
                Session["AgendaCountS"] = countRespAct;

                return RedirectToAction("PanelDetailsEditable", "Activities");
            }

            int count = RespAct();
            Session["AgendaCountS"] = count;

            return View();
        }

        public ActionResult PanelDetailsEditable()
        {
            Activity act = (Activity)TempData["Model"];
            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["ActivityDetails"] = act;
            ViewData["PanelAddedSpeaker"] = (List<User>)TempData["AddedSpeaker"];
            ViewData["PanelAddedResponsible"] = (List<User>)TempData["AddedResponsible"];
            ViewData["PanelAddedPlace"] = (List<Map>)TempData["AddedPlaces"];
            ViewData["Dates"] = StartEndTimes((Activity)TempData["Model"]);

            User Manager = db.User.Find(act.EventOrEventActivityForSurveyAndDiscussion);
            ViewData["Manager"] = Manager;

            return View();
        }

        public ActionResult PanelDetailsOrg(Guid IDofActivity, string from, Guid? FromSpeaker)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            var EA = db.EventActivity.ToList();
            var ThisEA = EA.Where(t => t.ActivityID == act.Id).ToList();

            ViewData["Documents"] = ActivitiesDocument(ThisEA[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            ViewData["ActivityDetails"] = act;
            ViewData["PanelAddedSpeaker"] = AddedSpeaker;
            ViewData["PanelAddedResponsible"] = AddedResponsible;
            ViewData["PanelAddedPlace"] = AddedPlaces;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["From"] = from;

            User Manager = db.User.Find(act.EventOrEventActivityForSurveyAndDiscussion);
            ViewData["Manager"] = Manager;

            if (FromSpeaker != null)
            {
                ViewData["FromSpeaker"] = FromSpeaker;
            }
            else
            {
                ViewData["FromSpeaker"] = null;
            }
            return View();
        }

        public ActionResult PanelEdit(Guid? IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);
            Guid PanelistID = act.EventOrEventActivityForSurveyAndDiscussion;

            TempData["spkrs"] = AddedSpeaker;
            TempData["rspnbls"] = AddedResponsible;
            TempData["plcs"] = AddedPlaces;
            TempData["act"] = act;
            Session["PanelBackButton"] = "BackToList";
            string edit = "edit";

            return RedirectToAction("CreatePanel", new { edit = edit, DeleteAfterEdit = IDofActivity, Panelist = PanelistID });
        }

        public ActionResult DeletePanel(Guid ActId)
        {
            Guid deletePrevious = ActId;
            if (deletePrevious.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(deletePrevious);
                activity.IsActive = false;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult PanelDetails(Guid IDofActivity)
        {
            // EventActivity'i aldık
            var AllEventActivities = db.EventActivity.ToList();
            var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();

            // AddedSpeakers
            var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
            var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedSpeaker = new List<User>();
            for (int i = 0; i < SpeakerList.Count; i++)
            {
                User speaker = db.User.Find(SpeakerList[i].SpeakerID);
                AddedSpeaker.Add(speaker);
            }

            // AddedResponsibles
            var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
            var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
            List<User> AddedResponsible = new List<User>();
            for (int i = 0; i < ResponsibleList.Count; i++)
            {
                User responsible = db.User.Find(ResponsibleList[i].ResponsibleID);
                AddedResponsible.Add(responsible);
            }

            // AddedPlaces
            var AllAddedPlaceList = db.MapEventActivity.ToList();
            var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
            List<Map> AddedPlaces = new List<Map>();
            for (int i = 0; i < PlaceList.Count; i++)
            {
                Map place = db.Map.Find(PlaceList[i].MapID);
                AddedPlaces.Add(place);
            }

            Activity act = db.Activity.Find(EditEventActivityID[0].ActivityID);

            bool scheduled = isScheduled(IDofActivity);

            ViewData["isScheduled"] = scheduled;
            ViewData["PanelAddedSpeaker"] = AddedSpeaker;
            ViewData["PanelAddedResponsible"] = AddedResponsible;
            ViewData["PanelAddedPlace"] = AddedPlaces;
            ViewData["ActivityDetails"] = act;
            ViewData["Dates"] = StartEndTimes(act);
            ViewData["Documents"] = ActivitiesDocument(EditEventActivityID[0].Id);
            ViewData["TypeOfDoc"] = TempData["TypeOfDoc"];

            User Manager = db.User.Find(act.EventOrEventActivityForSurveyAndDiscussion);
            ViewData["Manager"] = Manager;

            return View();
        }

        public ActionResult RecoverPanel(Guid IDofActivity)
        {
            Activity activity = db.Activity.Find(IDofActivity);
            activity.IsActive = true;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }

        public ActionResult DeletePanelPermanently(Guid IDofActivity)
        {
            if (IDofActivity.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Activity activity = db.Activity.Find(IDofActivity);
                db.Activity.Remove(activity);
                db.SaveChanges();

                // EventActivity'den de siliniecek
                var AllEventActivities = db.EventActivity.ToList();
                var EditEventActivityID = AllEventActivities.Where(k => k.ActivityID == IDofActivity).ToList();
                Guid oldEventActivityID = EditEventActivityID[0].Id;
                EventActivity deleteEA = db.EventActivity.Find(oldEventActivityID);
                db.EventActivity.Remove(deleteEA);
                db.SaveChanges();

                var AllAgendaList = db.AgendaEventActivity.ToList();
                var deletefromagenda = AllAgendaList.Where(t => t.EventActivityID == oldEventActivityID).ToList();
                AgendaEventActivity isdeleting = db.AgendaEventActivity.Find(deletefromagenda[0].Id);
                db.AgendaEventActivity.Remove(isdeleting);
                db.SaveChanges();

                // Delete from SpeakerAEventActivity
                var AllAddedSpeakerList = db.SpeakerEventActivity.ToList();
                var SpeakerList = AllAddedSpeakerList.Where(l => l.EventActivityID == oldEventActivityID).ToList();
                for (int i = 0; i < SpeakerList.Count; i++)
                {
                    SpeakerEventActivity deleteSpeaker = db.SpeakerEventActivity.Find(SpeakerList[i].Id);
                    db.SpeakerEventActivity.Remove(deleteSpeaker);
                    db.SaveChanges();
                }

                // Delete from ResponsibleEventActivity
                var AllAddedResponsibleList = db.ResponsibleEventActivity.ToList();
                var ResponsibleList = AllAddedResponsibleList.Where(l => l.EventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < ResponsibleList.Count; i++)
                {
                    ResponsibleEventActivity deleteResponsible = db.ResponsibleEventActivity.Find(ResponsibleList[i].Id);
                    db.ResponsibleEventActivity.Remove(deleteResponsible);
                    db.SaveChanges();
                }

                // Delete from MapEventActivity
                var AllAddedPlaceList = db.MapEventActivity.ToList();
                var PlaceList = AllAddedPlaceList.Where(l => l.EventOrEventActivityID == EditEventActivityID[0].Id).ToList();
                for (int i = 0; i < PlaceList.Count; i++)
                {
                    MapEventActivity deleteMap = db.MapEventActivity.Find(PlaceList[i].Id);
                    db.MapEventActivity.Remove(deleteMap);
                    db.SaveChanges();
                }
            }

            int countRespAct = RespAct();
            Session["AgendaCountS"] = countRespAct;

            return RedirectToAction("ActivityListEditable", "Activities");
        }




























































































































































































        // GET: Activities
        public ActionResult Index()
        {
            return View(db.Activity.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ActivityCategoryID,Name,StartDateTime,EndDateTime,Summary,Content,IsActive,EventOrEventActivityForSurveyAndDiscussion,IsPostEventSurvey")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                activity.Id = Guid.NewGuid();
                db.Activity.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ActivityCategoryID,Name,StartDateTime,EndDateTime,Summary,Content,IsActive,EventOrEventActivityForSurveyAndDiscussion,IsPostEventSurvey")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Activity activity = db.Activity.Find(id);
            db.Activity.Remove(activity);
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
