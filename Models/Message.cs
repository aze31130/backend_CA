using System;

namespace backend_CA.Models
{
    public class Message
    {
        public int id { get; set; }
        public int roomId { get; set; }
        public string content { get; set; }
        //public DateTime written { get; set; }
        //DateTime.UtcNow
        public bool isRead { get; set; }
    }
}
