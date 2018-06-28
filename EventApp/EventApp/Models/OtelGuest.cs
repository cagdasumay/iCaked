using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class OtelGuest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid UserOtelID { get; set; }
        public string ActivationCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }
        public bool CheckIn { get; set; }
        public bool Reservation { get; set; }

        [DataType(DataType.PhoneNumber)]
        public String Phone { get; set; }

        public int CountryPhoneCode { get; set; }
    }
}