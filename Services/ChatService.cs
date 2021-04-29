using backend_CA.Data;
using backend_CA.Models;
using System;
using System.Linq;

namespace backend_CA.Services
{
    public interface IChatService
    {
        bool isRoomIdExist(int roomId);
        bool isRoomReadable(int roomId, int userId);
        bool isUserExist(int userId);
    }

    public class ChatService : IChatService
    {
        private Context _context;
        public ChatService(Context context)
        {
            _context = context;
        }

        //-----
        //Returns true if the room id exists
        //-----
        public bool isRoomIdExist(int roomId)
        {
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)) == null)
            {
                return false;
            }
            return true;
        }

        //-----
        //Returns true if the given user can read the room
        //-----
        public bool isRoomReadable(int roomId, int userId)
        {
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)).senderId.Equals(userId)
                || _context.chats.ToList().Find(x => x.id.Equals(roomId)).receiverId.Equals(userId))
            {
                return true;
            }
            return false;
        }

        //-----
        //Returns true if the given user id exists
        //-----
        public bool isUserExist(int userId)
        {
            if (_context.users.ToList().Find(x => x.id.Equals(userId)) == null)
            {
                return false;
            }
            return true;
        }
    }
}
