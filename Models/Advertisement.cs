using System;
using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class Advertisement
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime posted { get; set; }
        public bool isBanned { get; set; }
    }
}
