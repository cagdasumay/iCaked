using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace Cake.Models
{
    public class BlogFeed
    {
        public BlogFeed()
        {
            Posts = new List<SyndicationItem>();
        }

        public List<SyndicationItem> Posts { get; set; }
    }
}