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
        void createRoom(int ownerId, string roomName, string roomDescription);
        void deleteRoom(int roomId, int userId);
        List<Chat> getRooms(int userId);
        List<User> getRoomMember(int roomId);
        List<Message> getChatMessages(int roomId, int limit, int userId);
        void sendMessage(int roomId, int senderId, string content);
        void inviteUser(int userId, int roomId);
        void leaveChat(int userId, int roomId);
        void kickChat(int userId, int roomId, int executerId);
    }

    public class ChatService : IChatService
    {
        private Context _context;
        public ChatService(Context context)
        {
            _context = context;
        }

        public List<Chat> getRooms(int userId)
        {
            List<Chat> chats = new List<Chat> { };
            foreach (UsersRooms room in _context.usersrooms.ToList().FindAll(x => x.userId.Equals(userId)))
            {
                chats.Add(getChatById(room.roomId));
            }

            return chats;
        }

        public Chat getChatById(int chatId)
        {
            return _context.chats.ToList().Find(x => x.id.Equals(chatId));
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
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)).owner.Equals(userId))
            {
                return true;
            }

            foreach (UsersRooms ur in _context.usersrooms.ToList().FindAll(x => x.roomId.Equals(roomId)))
            {
                if (ur.userId.Equals(userId))
                {
                    return true;
                }
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
        public void createRoom(int ownerId, string roomName, string roomDescription)
        {
            if (!(isUserExist(ownerId)))
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

            _context.chats.Add(chat);

            //We need to apply the changes here to make the id property of the chat object to update
            //to the foreign key
            _context.SaveChanges();

            //Add the user to the usersrooms
            inviteUser(ownerId, _context.chats.ToList().Find(x => x.id.Equals(chat.id)).id);
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

            //Remove all usersrooms
            foreach (UsersRooms ur in _context.usersrooms.ToList())
            {
                if (ur.roomId.Equals(roomId))
                {
                    _context.usersrooms.Remove(ur);
                }
            }
            _context.SaveChanges();
        }

        //-----
        //Invites a user to a chat room
        //-----
        public void inviteUser(int userId, int roomId)
        {
            //Check if the user exists
            if (!isUserExist(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Check if the room exists
            if (!isRoomExist(roomId))
            {
                throw new CustomException("This room doesn't exist ! ");
            }

            UsersRooms ur = new UsersRooms();
            ur.roomId = roomId;
            ur.userId = userId;
            _context.usersrooms.Add(ur);
            _context.SaveChanges();
        }


        //-----
        //Leaves a chat room
        //-----
        public void leaveChat(int userId, int roomId)
        {
            //Check if the user exists
            if (!isUserExist(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Check if the room exists
            if (!isRoomExist(roomId))
            {
                throw new CustomException("This room doesn't exist ! ");
            }

            //If the user is the owner then the room is destroyed
            if (_context.chats.ToList().Find(x => x.id.Equals(roomId)).owner.Equals(_context.users.ToList().Find(x => x.id.Equals(userId)).id))
            {
                deleteRoom(roomId, userId);
            }
            else
            {
                UsersRooms ur = _context.usersrooms.ToList().Find(x => x.roomId.Equals(roomId) && x.userId.Equals(userId));
                _context.usersrooms.Remove(ur);
                _context.SaveChanges();
            }
        }

        //-----
        //Kicks someone from a chat room
        //-----
        public void kickChat(int userId, int roomId, int executerId)
        {
            //Check if the user exists
            if (!isUserExist(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Check if the room exists
            if (!isRoomExist(roomId))
            {
                throw new CustomException("This room doesn't exist !");
            }

            //If the user is not the owner
            if (!_context.chats.ToList().Find(x => x.id.Equals(roomId)).owner.Equals(_context.users.ToList().Find(x => x.id.Equals(executerId)).id))
            {
                throw new CustomException("Only the owner is able to kick someone !");
            }

            UsersRooms ur = _context.usersrooms.ToList().Find(x => x.roomId.Equals(roomId) && x.userId.Equals(userId));
            _context.usersrooms.Remove(ur);
            _context.SaveChanges();
        }
    }
}
