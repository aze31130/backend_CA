using System.Collections.Generic;

namespace backend_CA.Models
{
    public class Job
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int availableSlots { get; set; }
        public int employerId { get; set; }
    }
}
