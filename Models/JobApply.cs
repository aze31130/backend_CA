using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class JobApply
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public int jobId { get; set; }
        public bool isAccepted { get; set; }
    }
}
