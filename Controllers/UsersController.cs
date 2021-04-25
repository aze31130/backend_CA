using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        public IConfiguration Configuration;
        private IUserService _userService;
        private readonly Context _context;
        public UsersController(Context context, IUserService userService, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            Configuration = configuration;
        }

        //-----
        //Function to list all users
        //-----
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.users.ToListAsync();
        }

        //-----
        //Function to register
        //-----
        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<User> Register(RegisterModel model)
        {
            try
            {
                _userService.Register(model);
                return Ok(new { message = "Account successfully registered !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Function to login
        //-----
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                User user = _userService.Authenticate(model.username, model.password);

                if (user == null)
                {
                    return BadRequest(new { message = "Username or password is invalid !" });
                }

                user.lastlogin = DateTime.UtcNow;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Configuration["Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.id.ToString()),
                    new Claim(ClaimTypes.Role, user.adminLevel.ToString() ?? "null")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info and authentication token
                return Ok(new
                {
                    id = user.id,
                    username = user.username,
                    firstName = user.firstname,
                    lastName = user.lastname,
                    Token = tokenString
                });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        [HttpPut("id")]
        public async Task<ActionResult> UpdateUser(int id, User user)
        {
            int userId = getUserId();
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

        //-----
        //Bans a user
        //-----
        [Authorize(Roles = AdminLevel.Administrator)]
        [HttpPost("ban")]
        public async Task<ActionResult> BanUser(int userId)
        {
            if (_userService.isUserIdValid(userId))
            {
                User user = _userService.GetUserById(userId);
                user.isBanned = true;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully banned user " + userId });
            }
            return BadRequest(new { message = "The user id is invalid !" });
        }

        //-----
        //Forgives a user
        //-----
        [Authorize(Roles = AdminLevel.Administrator)]
        [HttpPost("forgive")]
        public async Task<ActionResult> ForgiveUser(int userId)
        {
            if (_userService.isUserIdValid(userId))
            {
                User user = _userService.GetUserById(userId);
                user.isBanned = false;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully forgiven user " + userId });
            }
            return BadRequest(new { message = "The user id is invalid !" });
        }

        //-----
        //Add a skill to the user
        //-----
        [HttpPost("AddSkill")]
        public async Task<ActionResult> AddSkill(SKILLS skill)
        {
            int userId = getUserId();
            if (!_userService.isUserIdValid(userId))
            {
                return BadRequest(new { message = "The user id is invalid !" });
            }
            Skill s = new Skill();
            s.jobId = -1;
            s.userId = userId;
            s.skill = skill;
            _context.skills.Add(s);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Successfully added the skill " + skill});
        }

        //-----
        //Add a skill to the user
        //-----
        [HttpPost("RemoveSkill")]
        public async Task<ActionResult> RemoveSkill(SKILLS skill)
        {
            int userId = getUserId();
            if (!_userService.isUserIdValid(userId))
            {
                return BadRequest(new { message = "The user id is invalid !" });
            }

            //find the skill
            //Skill s = _userService.findSkill();
            //remove it

            await _context.SaveChangesAsync();
            return Ok(new { message = "Successfully deleted the skill " + skill });
        }

        [Authorize(Roles = AdminLevel.Administrator)]
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

        //-----
        //Returns the id of the currently logged account
        //-----
        private int getUserId()
        {
            return int.Parse(User.Identity.Name);
        }
    }
}
