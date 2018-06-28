using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventApp.Models
{
    public class Map
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid MapTypeID { get; set; }
        public string Name { get; set; }
        [DataType(DataType.ImageUrl)]
        public string MapDirectory { get; set; }
        public double? Lattitude { get; set; }
        public double? Longitude { get; set; }
        public string Usage { get; set; }
    }
}