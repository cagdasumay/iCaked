using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class SurveyActivityQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid ActivityID { get; set; }
        public Guid SurveyQuestionTypeID { get; set; }
        public string Quesiton { get; set; }
        public int Order { get; set; }
    }
}