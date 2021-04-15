using backend_CA.Data;
using backend_CA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatsController : Controller
    {
        private readonly Context _context;
        public ChatsController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetChatMessages(int roomId)
        {
            List<Message> messages = new List<Message> { };
            foreach (Message m in _context.messages.ToList())
            {
                if (m.roomId.Equals(roomId))
                {
                    messages.Append(m);
                }
            }
            return messages;
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Need to check if the sender / receiver id is valid
            _context.messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatMessages", new { id = message.id }, message);
        }
    }
}
