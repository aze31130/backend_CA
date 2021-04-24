using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class Job
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool isPremiumOnly { get; set; }
        public int availableSlots { get; set; }
        public int employerId { get; set; }
    }
}
