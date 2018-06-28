using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        [DataType(DataType.Password)]
        public String EventConfirmation { get; set; }
        [DataType(DataType.ImageUrl)]
        public String EventLogo { get; set; }

        public double? Lattitude { get; set; }
        public double? Longitude { get; set; }
        public string Content { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Linkedin { get; set; }
        public string Website { get; set; }
        public string Youtube { get; set; }
        public string Pinterest { get; set; }
        public string GooglePlus { get;set; }
        public string Tumblr { get;set; }
    }
}