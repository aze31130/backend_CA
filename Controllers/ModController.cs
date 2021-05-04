using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace backend_CA.Controllers
{
    [Authorize(Roles = AdminLevel.Moderator + "," + AdminLevel.Administrator)]
    [ApiController]
    [Route("[controller]")]
    public class ModController : Controller
    {
        public IConfiguration Configuration;
        private IAdminService _adminService;
        private readonly Context _context;
        public ModController(Context context, IAdminService adminService, IConfiguration configuration)
        {
            _context = context;
            _adminService = adminService;
            Configuration = configuration;
        }

        //-----
        //Bans a user
        //-----
        [HttpPost("ban")]
        public ActionResult BanUser(int userId)
        {
            try
            {
                _adminService.banUser(userId);
                return Ok(new { message = "Successfully banned user " + userId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Forgives a user
        //-----
        [HttpPost("forgive")]
        public ActionResult ForgiveUser(int userId)
        {
            try
            {
                _adminService.forgiveUser(userId);
                return Ok(new { message = "Successfully banned user " + userId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
