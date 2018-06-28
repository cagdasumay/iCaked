using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid ActivityCategoryID { get; set; }
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EndDateTime { get; set; }

        public string Summary { get; set; }
        public string Content { get; set; }
        public bool? IsActive { get; set; }
        public Guid EventOrEventActivityForSurveyAndDiscussion { get; set; }
        // Private Chat için de burada kimse seçilmeden oluşturulur, sonra adam ekleme bölümünde kişi eklendikçe UserEventActivity'e eklenir
        public bool? IsPostEventSurvey { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
}