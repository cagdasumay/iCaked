using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EventApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Login",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "UserDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "UserDetails", id = "modelItem => item.Id" }
            );
            routes.MapRoute(
                name: "AttendeeInfo",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "AttenderDetails"}
            );
            routes.MapRoute(
                name: "MapDisplay",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Maps", action = "MapDisplay"}
            );
            routes.MapRoute(
                name: "DisplayDocument",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Documents", action = "DisplayDocument" }
            );
            routes.MapRoute(
                name: "GMapDisplay",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Maps", action = "GMapDisplay" }
            );
            routes.MapRoute(
                name: "SpeakerDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "SpeakerDetails" }
            );
            routes.MapRoute(
                name: "UserAccomodation",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "UserAccomodation" }
            );
            routes.MapRoute(
                name: "InfoBoothDetailsOrg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "InfoBooths", action = "InfoBoothDetailsOrg" }
            );
            routes.MapRoute(
                name: "ApproveOtelApplication",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "ApproveOtelApplication" }
            );
            routes.MapRoute(
                name: "UserCheckInForm",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "UserCheckInForm" }
            );
            routes.MapRoute(
                name: "GuestCheckInForm",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "GuestCheckInForm" }
            );
            routes.MapRoute(
                name: "InfoBoothDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "InfoBooths", action = "InfoBoothDetails" }
            );
            routes.MapRoute(
                name: "SpeakerDetailsOrg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "SpeakerDetailsOrg" }
            );
            routes.MapRoute(
                name: "OtelDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "OtelDetails" }
            );           
            routes.MapRoute(
                name: "OtelDetailsOrg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Events", action = "OtelDetailsOrg" }
            );
            routes.MapRoute(
                name: "AttenderDetailsOrg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "AttenderDetailsOrg" }
            );
            routes.MapRoute(
                name: "EditSurvey",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "EditSurvey" }
            );
            routes.MapRoute(
                name: "SurveyDisplayOrg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "SurveyDisplayOrg" }
            );
            routes.MapRoute(
                name: "AttendSurvey",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "AttendSurvey" }
            );
            routes.MapRoute(
                name: "ConferenceDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "ConferenceDetails" }
            );
            routes.MapRoute(
                name: "PanelDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "PanelDetails" }
            );
            routes.MapRoute(
                name: "PartnerDetails",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Partners", action = "PartnerDetails" }
            );
            routes.MapRoute(
                "Error",
                "{*url}",
                new { controller = "Error", action = "Http404" }
            );
            routes.MapRoute(
                name: "DeleteMap",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Maps", action = "Delete", id = "Guid id" }
            );

            routes.MapRoute(
                name: "EventEditProfile",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Event", action = "EventEditProfile" }
            );
            routes.MapRoute(
                name: "ConferenceEdit",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Activities", action = "ConferenceEdit" }
            );
            routes.MapRoute(
               name: "EventInfo",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Event", action = "EventInfo" }
            );



        }
    }
}
