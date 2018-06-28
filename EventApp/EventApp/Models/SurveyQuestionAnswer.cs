using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class SurveyQuestionAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid SurveyActivityQuestionID { get; set; }
        public Guid UserID { get; set; }
        public Guid SurveyQuestionOptionID { get; set; }
        public string Answer { get; set; }
    }
}