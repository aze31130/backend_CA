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
        private readonly IEmailService _emailService;
        private readonly Context _context;
        public UsersController(Context context, IUserService userService, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _emailService = emailService;
            Configuration = configuration;
        }

        //-----
        //Function to get every ads
        //-----
        [HttpPost("GetAds")]
        public ActionResult<IEnumerable<Advertisement>> GetAds()
        {
            return _context.advertisements.ToList().FindAll(x => x.isBanned.Equals(false));
        }

        //-----
        //Function to post an advertisement
        //-----
        [HttpPost("PostAdvertisement")]
        public ActionResult PostAdvertisement(AdvertisementModel model)
        {
            try
            {
                _userService.PostAd(getUserId(), model);
                return Ok(new { message = "Successfully posted ad : " + model.title });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to edit an advertisement
        //-----
        [HttpPut("EditAdvertisement")]
        public ActionResult EditAdvertisement(int adId, AdvertisementModel model)
        {
            try
            {
                _userService.EditAd(getUserId(), adId, model);
                return Ok(new { message = "Successfully edited ad : " + adId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to edit an advertisement
        //-----
        [HttpDelete("DeleteAdvertisement")]
        public ActionResult DeleteAdvertisement(int adId)
        {
            try
            {
                _userService.DeleteAd(getUserId(), adId);
                return Ok(new { message = "Successfully deleted ad :" + adId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to open a ticket
        //-----
        [HttpPost("OpenTicket")]
        public ActionResult OpenTicket(OpenTicketModel model)
        {
            try
            {
                _userService.OpenTicket(getUserId(), model);
                return Ok(new { message = "Successfully opened ticket :" + model.title + ". An Administrator will answer you in a few." });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to list all account tickets
        //-----
        [HttpPost("GetAllTickets")]
        public ActionResult<IEnumerable<Ticket>> GetAllTickets()
        {
            return _context.tickets.ToList().FindAll(x => x.userId.Equals(getUserId()));
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
                _userService.Register(getUserId(), model);
                return Ok(new { message = "Account successfully registered !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to change selfuser password
        //-----
        [HttpPut("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                _userService.changeUserPassword(getUserId(), model);
                return Ok(new { message = "Successfully changed the password !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to resend a password
        //-----
        [AllowAnonymous]
        [HttpPut("ForgotPassword")]
        public ActionResult ForgotPassword(string email)
        {
            try
            {
                _userService.ForgotPassword(email);
                return Ok(new { message = "Successfully changed the password !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
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

                //Updated last login
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
                    new Claim(ClaimTypes.Role, user.type.ToString() ?? "null")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                //Return basic user info and authentication token
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

        //-----
        //Function to update self user
        //-----
        [HttpPut("UpdateUser")]
        public ActionResult UpdateUser(UpdateProfileModel model)
        {
            try
            {
                _userService.updateUser(getUserId(), model);
                return Ok(new { message = "Profile successfully updated !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Add a skill to the user
        //-----
        [HttpPost("AddSkill")]
        public ActionResult AddSkill(SKILLS skill)
        {
            try
            {
                _userService.addUserSkill(getUserId(), skill);
                return Ok(new { message = "Successfully added the skill " + skill.ToString() });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Add a skill to the user
        //-----
        [HttpPost("RemoveSkill")]
        public ActionResult RemoveSkill(SKILLS skill)
        {
            try
            {
                _userService.removeUserSkill(getUserId(), skill);
                return Ok(new { message = "Successfully deleted the skill " + skill.ToString() });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Get a list of all user's skills
        //-----
        [HttpGet("GetSelfUserSkills")]
        public ActionResult<IEnumerable<SKILLS>> GetSelfUserSkills()
        {
            return GetUserSkills(getUserId());
        }

        //-----
        //Apply to a Job
        //-----
        [HttpPost("apply")]
        public ActionResult<Job> Apply(int jobId)
        {
            try
            {
                _userService.Apply(getUserId(), jobId);
                return Ok(new { message = "you successfully applied to this job" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Get a list of all skills for a given user
        //-----
        [HttpGet("GetUserSkills")]
        public ActionResult<IEnumerable<SKILLS>> GetUserSkills(int userId)
        {
            try
            {
                return _userService.GetUserSkills(userId);
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //List all available Jobs
        //-----
        [HttpPost("GetAvailableJobs")]
        public ActionResult<IEnumerable<Job>> GetAllAvailableJobs()
        {
            return _context.jobs.ToList().FindAll(x => x.availableSlots > 0);
        }

        //-----
        //Returns the id of the currently logged account
        //-----
        private int getUserId()
        {
            try
            {
                int userId = int.Parse(User.Identity.Name);
                if (_userService.isUserIdValid(userId))
                {
                    return userId;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
