using System.Collections.Generic;

namespace backend_CA.Models
{
    public class Chat
    {
        public int roomId { get; set; }
        public List<Message> messageList { get; set; }
        public int senderId { get; set; }
        public int rereceiverId { get; set; }
    }
}
