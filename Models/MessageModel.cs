using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class MessageModel
    {
        [Required]
        public int roomId { get; set; }
        [Required]
        public string content { get; set; }
    }
}
