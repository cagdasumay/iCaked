using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class UserEventActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid UserEventID { get; set; }
        public Guid EventActivityID { get; set; }
        public bool? IsActive { get; set; }
        public string AttendTime { get; set; }
        public string AttendDay { get; set; }
    }
}