using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class SurveyQuestionOption
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid SurveyActivityQuestionID { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
    }
}