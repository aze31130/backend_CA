using backend_CA.Data;
using backend_CA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly Context _context;
        public ChatController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChatRooms()
        {
            return await _context.chats.ToListAsync();
        }

        [HttpPost("roomId")]
        public async Task<ActionResult<Message>> SendMessage(int roomId, Message message)
        {
            if ((!ModelState.IsValid) || !_context.chats.Any(x => x.roomId.Equals(roomId)))
            {
                return BadRequest(ModelState);
            }
            Chat room = _context.chats.Find(roomId);
            room.messageList.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRooms", new { id = message.uuid }, message);

        }
    }
}
