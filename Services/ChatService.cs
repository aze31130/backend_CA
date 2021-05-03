using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend_CA.Services
{
    public interface IChatService
    {
        bool isRoomExist(int roomId);
        bool isRoomReadable(int roomId, int userId);
        bool isUserExist(int userId);
        bool isRoomOwned(int roomId, int userId);
        void createRoom(int ownerId, string roomName, string roomDescription, int receiverId);
        void deleteRoom(int roomId, int userId);
        List<User> getRoomMember(int roomId);
        List<Message> getChatMessages(int roomId, int limit, int userId);
        void sendMessage(int roomId, int senderId, string content);
    }

    public class ChatService : IChatService
    {
        private Context _context;
        public ChatService(Context context)
        {
            _context = context;
        }

        public void sendMessage(int roomId, int senderId, string content)
        {
            if (!isRoomExist(roomId))
            {
                throw new CustomException("Cannot send your message ! Room Id: " + roomId + " doesn't exist !");
            }

            if (!isRoomReadable(roomId, senderId))
            {
                throw new CustomException("You cannot send a message to this room");
            }

            Message message = new Message();
            message.roomId = roomId;
            message.senderId = senderId;
            message.content = content;
            message.written = DateTime.UtcNow;
            message.isRead = false;
            _context.messages.Add(message);
            _context.SaveChanges();
        }

        //-----
        //Returns a list of message in a room
        //-----
        public List<Message> getChatMessages(int roomId, int limit, int userId)
        {
            if (!isRoomExist(roomId))
            {
                throw new CustomException("This room id doesn't exists");
            }

            if (!isRoomReadable(roomId, userId))
            {
                throw new CustomException("You cannot read this room");
            }

            List<Message> messages = new List<Message> { };
            if (limit > 0)
            {
                messages = _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId)).Take(limit).ToList();
            }
            else
            {
                messages = _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId));
            }

            //Mark every requested messages as read
            foreach (Message m in messages)
            {
                if (!m.senderId.Equals(userId))
                {
                    m.isRead = true;
                    _context.Entry(m).State = EntityState.Modified;
                }
            }

            _context.SaveChanges();
            return messages;
        }

        //-----
        //Returns true if the room id exists
        //-----
        public bool isRoomExist(int roomId)
        {
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)) == null)
            {
                return false;
            }
            return true;
        }

        //-----
        //Returns the user object when 
        //-----
        private User getUserById(int userId)
        {
            return _context.users.ToList().Find(x => x.id.Equals(userId));
        }

        //-----
        //Returns a list of users inside a chat room
        //-----
        public List<User> getRoomMember(int roomId)
        {
            List<User> users = new List<User> { };
            foreach (UsersRooms ur in _context.usersrooms.ToList())
            {
                if (ur.roomId.Equals(roomId))
                {
                    users.Add(getUserById(ur.userId));
                }
            }
            return users;
        }

        //-----
        //Returns true if the room is owned by a given user
        //-----
        public bool isRoomOwned(int roomId, int userId)
        {
            if (!isRoomExist(roomId))
            {
                return false;
            }

            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)).owner.Equals(userId))
            {
                return true;
            }
            return false;
        }

        //-----
        //Returns true if the given user can read the room
        //-----
        public bool isRoomReadable(int roomId, int userId)
        {
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)).owner.Equals(userId)
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

        //-----
        //Creates a chat room
        //-----
        public void createRoom(int ownerId, string roomName, string roomDescription, int receiverId)
        {
            if (!(isUserExist(ownerId) && isUserExist(receiverId)))
            {
                throw new CustomException("Receiver or sender Id are incorrect !");
            }

            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(roomDescription))
            {
                throw new CustomException("You need to provide a room name and description !");
            }

            Chat chat = new Chat();
            chat.owner = ownerId;
            chat.roomName = roomName;
            chat.roomDescription = roomDescription;
            chat.receiverId = receiverId;

            _context.chats.Add(chat);
            _context.SaveChanges();
        }

        //-----
        //Deletes a chat room
        //-----
        public void deleteRoom(int roomId, int userId)
        {
            //Check if the user is the owner
            if (!isRoomOwned(roomId, userId))
            {
                throw new CustomException("You cannot delete this room !");
            }

            Chat room = _context.chats.ToList().Find(x => x.id.Equals(roomId));
            //Remove the room
            _context.chats.Remove(room);

            //Remove every messages in the room
            foreach (Message m in _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId)))
            {
                if (m.roomId.Equals(roomId))
                {
                    _context.messages.Remove(m);
                }
            }
            _context.SaveChanges();
        }
    }
}
