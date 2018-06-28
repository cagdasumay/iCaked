using System;

namespace Cake.Models
{
    public class Promotion
    {
        public Guid code { get; set; }
        public bool isOk { get; set; }
        public string newCartPrice { get; set; }
        public string indirimTutari { get; set; }
        public string append { get; set; }
        public int newCount { get; set; }
    }
}