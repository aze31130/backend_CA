using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ChatsController : Controller
    {
        private IChatService _chatService;
        private readonly Context _context;
        public ChatsController(Context context, IChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        //-----
        //Method to get a message
        //-----
        [HttpGet("getMessages")]
        public ActionResult<IEnumerable<Message>> GetChatMessages(int roomId, int limit)
        {
            if (!_chatService.isRoomIdExist(roomId))
            {
                return BadRequest(new { message = "This room id doesn't exists" });
            }

            if (!_chatService.isRoomReadable(roomId, getUserId()))
            {
                return BadRequest(new { message = "You cannot read this room" });
            }

            if (limit > 0)
            {
                return _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId)).Take(limit).ToList();
            }
            return _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId));
        }

        //-----
        //Method to send a message
        //-----
        [HttpPost("sendMessage")]
        public async Task<ActionResult<Message>> SendMessage(MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_chatService.isRoomIdExist(model.roomId))
            {
                return BadRequest(new { message = "Cannot send your message ! Room Id: " + model.roomId + " doesn't exist !" });
            }

            if (!_chatService.isRoomReadable(model.roomId, getUserId()))
            {
                return BadRequest(new { message = "You cannot send a message to this room" });
            }

            Message message = new Message();
            message.roomId = model.roomId;
            message.content = model.content;
            message.written = DateTime.UtcNow;
            message.isRead = false;
            _context.messages.Add(message);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetChatMessages", new { id = message.id }, message);
        }

        //-----
        //Method to create a room
        //-----
        [HttpPost("createRoom")]
        public async Task<ActionResult> CreateChatRoom(CreateRoomModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_chatService.isUserExist(getUserId()) && _chatService.isUserExist(model.receiverId))
            {
                Chat chat = new Chat();
                chat.senderId = getUserId();
                chat.receiverId = model.receiverId;

                _context.chats.Add(chat);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sucessfully created the room !" });
            }
            return BadRequest(new { message = "Receiver or sender Id are incorrect !" });
        }

        //-----
        //Returns the id of the currently logged account
        //-----
        private int getUserId()
        {
            return int.Parse(User.Identity.Name);
        }
    }
}
