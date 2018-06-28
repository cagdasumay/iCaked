using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventApp.EventApp
{
    public class EventAppInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<EventAppContext>
    {
        protected override void Seed(EventAppContext context)
        {
            base.Seed(context);
        }
    }
}