using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class Otels
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid EventID { get; set; }
        public string Name { get; set; }
        public int StarNumber { get; set; }
        public string OtelContent { get; set; }
        public string Website { get; set; }

        [DataType(DataType.ImageUrl)]
        public String Photo { get; set; }
        public double? Lattitude { get; set; }
        public double? Longitude { get; set; }
        public string Currency { get; set; }
        public bool isPriceDisplay { get; set; }
        public double OnePersonPrice { get; set; }
        public double TwoPersonPrice { get; set; }
        public double ThreePersonPrice { get; set; }
        public int PhotoNumber { get; set; }
    }
}