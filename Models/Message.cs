using System;

namespace backend_CA.Models
{
    public class Message
    {
        public string id { get; set; }
        public string roomId { get; set; }
        public string content { get; set; }
        public DateTime written { get; set; }
        public bool isRead { get; set; }
    }
}
