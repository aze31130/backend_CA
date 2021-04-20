namespace backend_CA.Models
{
    public class Chat
    {
        public int id { get; set; } //used as roomId
        public int senderId { get; set; }
        public int receiverId { get; set; }
    }
}
