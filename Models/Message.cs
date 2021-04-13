using System;

namespace backend_CA.Models
{
    public class Message
    {
        public string uuid { get; set; }
        public string content { get; set; }
        public DateTime written { get; set; }
        public bool isRead { get; set; }
    }
}
