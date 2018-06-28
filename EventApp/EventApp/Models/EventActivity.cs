using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class EventActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid EventID { get; set; }
        public Guid ActivityID { get; set; }
        public bool? IsActive { get; set; }
    }
}