using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace backend_CA.Controllers
{
    [Authorize(Roles = AdminLevel.Administrator)]
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        public IConfiguration Configuration;
        private IAdminService _adminService;
        private readonly Context _context;
        public AdminController(Context context, IAdminService adminService, IConfiguration configuration)
        {
            _context = context;
            _adminService = adminService;
            Configuration = configuration;
        }

        //-----
        //Function to get all non answered tickets
        //-----
        [HttpPost("GetTicketList")]
        public ActionResult<IEnumerable<Ticket>> GetTicketList()
        {
            return _context.tickets.ToList().FindAll(x => x.isClosed.Equals(false));
        }

        //-----
        //Function to answer to a ticket
        //-----
        [HttpPost("AnswerTicket")]
        public ActionResult AnswerTicket(int ticketId, string answer)
        {
            try
            {
                _adminService.answerTicket(ticketId, answer);
                return Ok(new { message = "The answer has been successfully recorded !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to update any user
        //-----
        [HttpPut("ForceUpdateUser")]
        public ActionResult ForceUpdateUser(int userId, UpdateProfileModel model)
        {
            try
            {
                _adminService.updateUser(userId, model);
                return Ok(new { message = "Profile successfully updated !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to hard delete a user
        //-----
        [HttpDelete("DeleteUser")]
        public ActionResult<User> DeleteUser(int userId)
        {
            try
            {
                _adminService.deleteUser(userId);
                return Ok(new { message = "Profile successfully deleted !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
