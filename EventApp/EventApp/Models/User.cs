using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid UserTypeID { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }

        [DataType(DataType.PhoneNumber)]
        public String Phone { get; set; }

        public string Company { get; set; }
        public string PersonelTitle { get; set; }
        public int CountryPhoneCode { get; set; }
        public string City { get; set; }
        public string About { get; set; }

        [DataType(DataType.ImageUrl)]
        public String PhotoDirectory { get; set; }
        //public bool? CheckIn { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Linkedin { get; set; }
        public string Website { get; set; }
        public string Youtube { get; set; }
        public string Pinterest { get; set; }
        public string GooglePlus { get; set; }
        public string Tumblr { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        public String AccountConfirmation { get; set; }
        public bool? IsActive { get; set; }
        public bool? ShowMyDetails { get; set; }
    }
}