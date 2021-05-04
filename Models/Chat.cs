using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class Chat
    {
        [Key]
        public int id { get; set; } //used as roomId
        public int owner { get; set; }
        public string roomName { get; set; }
        public string roomDescription { get; set; }
    }
}
