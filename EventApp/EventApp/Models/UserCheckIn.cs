using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class UserCheckIn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid UserOtelID { get; set; }
        public string ActivationCode { get; set; }
        public long NationalIdentityNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDay { get; set; }
        public string BirthDayStr { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CheckInDate { get; set; }
        public string CheckInStr { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }
        public string CheckOutStr { get; set; }

        [DataType(DataType.ImageUrl)]
        public String IDCardBackEnd { get; set; }
        [DataType(DataType.ImageUrl)]
        public String IDCardFrontEnd { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }

        [DataType(DataType.PhoneNumber)]
        public String Phone { get; set; }

        public int CountryPhoneCode { get; set; }

    }
}