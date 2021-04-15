using System.Collections.Generic;

namespace backend_CA.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int senderId { get; set; }
        public int receiverId { get; set; }
    }
}
