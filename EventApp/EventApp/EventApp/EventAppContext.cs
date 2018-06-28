using EventApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace EventApp.EventApp
{
    public class EventAppContext : DbContext
    {
        public EventAppContext() : base("EventAppContext") { }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<ActivityCategory> ActivityCategory { get; set; }
        public DbSet<AgendaEventActivity> AgendaEventActivity { get; set; }
        public DbSet<ChatDiscussionActivityWriting> ChatDiscussionActivityWriting { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentEventActivity> DocumentEventActivity { get; set; }
        public DbSet<DocumentFormat> DocumentFormat { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventActivity> EventActivity { get; set; }
        public DbSet<InfoBooth> InfoBooth { get; set; }
        public DbSet<InfoBoothEvent> InfoBoothEvent { get; set; }
        public DbSet<Map> Map { get; set; }
        public DbSet<MapEventActivity> MapEventActivity { get; set; }
        public DbSet<MapType> MapType { get; set; }
        public DbSet<Partner> Partner { get; set; }
        public DbSet<PartnerEvent> PartnerEvent { get; set; }
        public DbSet<ResponsibleEventActivity> ResponsibleEventActivity { get; set; }
        public DbSet<SpeakerEventActivity> SpeakerEventActivity { get; set; }
        public DbSet<SurveyActivityQuestion> SurveyActivityQuestion { get; set; }
        public DbSet<SurveyQuestionAnswer> SurveyQuestionAnswer { get; set; }
        public DbSet<SurveyQuestionOption> SurveyQuestionOption { get; set; }
        public DbSet<SurveyQuestionType> SurveyQuestionType { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserEvent> UserEvent { get; set; }
        public DbSet<UserEventActivity> UserEventActivity { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<PartnerEventCategory> PartnerEventCategory { get; set; }
        public DbSet<Otels> Otels { get; set; }
        public DbSet<UserOtel> UserOtels { get; set; }
        public DbSet<OtelGuest> OtelGuests { get; set; }
        public DbSet<UserCheckIn> UserCheckIn { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }

    }
}