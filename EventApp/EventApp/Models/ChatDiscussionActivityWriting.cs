using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class ChatDiscussionActivityWriting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid ActivityID { get; set; }
        public Guid UserID { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? PostDateTime { get; set; }

        public string PostDate { get; set; }
        public string Writing { get; set; }
    }
}