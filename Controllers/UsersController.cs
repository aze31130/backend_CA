using backend_CA.Data;
using backend_CA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly Context _context;
        public UsersController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.users.ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<User>> Add_User(User User)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.users.Add(User);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = User.id }, User);

        }
        [HttpPut("id")]
        public async Task<ActionResult> UpdateUser(int id, User user)
        {
            if (!id.Equals(user.id) || !_context.users.Any(x => x.id.Equals(id)))
            {
                return BadRequest();
            }
            else
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUsers", new { id = user.id }, user);
            }
        }
        [HttpDelete("id")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
        }
    }
}
