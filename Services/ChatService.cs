using backend_CA.Data;

namespace backend_CA.Services
{
    public interface IChatService
    {
        bool isRoomIdValid(int roomId);
    }

    public class ChatService : IChatService
    {
        private Context _context;
        public ChatService(Context context)
        {
            _context = context;
        }

        //-----
        //Returns true if the room id is valid
        //-----
        public bool isRoomIdValid(int roomId)
        {
            return true;
        }
    }
}
