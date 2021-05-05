using System;
using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class Ticket
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string answer { get; set; }
        public DateTime opened { get; set; }
        public bool isClosed { get; set; }
    }
}
