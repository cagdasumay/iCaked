using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class UserOtel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public Guid OtelID { get; set; }
        public string ActivationCode { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CheckInDate { get; set; }
        public string CheckInStr { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }
        public string CheckOutStr { get; set; }
        public int NumberOfGuest { get; set; }
        public bool CheckIn { get; set; }
        public bool Reservation { get; set; }
    }
}