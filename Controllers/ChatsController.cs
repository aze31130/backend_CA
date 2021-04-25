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

        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetChatMessages(int roomId, int limit)
        {
            if (limit > 0)
            {
                return _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId)).Take(limit).ToList();
            }
            return _context.messages.ToList().FindAll(x => x.roomId.Equals(roomId));
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_chatService.isRoomIdValid(model.roomId))
            {
                Message message = new Message();
                message.roomId = model.roomId;
                message.content = model.content;
                message.written = DateTime.UtcNow;
                message.isRead = false;
                _context.messages.Add(message);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetChatMessages", new { id = message.id }, message);
            }
            return BadRequest(new { message = "Cannot send your message ! Room Id: " + model.roomId + " doesn't exist !" });
        }
    }
}
