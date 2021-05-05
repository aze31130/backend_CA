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
        //Bans an ad
        //-----
        [HttpPost("BanAd")]
        public ActionResult BanAd(int adId)
        {
            try
            {
                _adminService.banAd(adId);
                return Ok(new { message = "Successfully banned ad " + adId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Forgives an ad
        //-----
        [HttpPost("ForgiveAd")]
        public ActionResult ForgiveAd(int adId)
        {
            try
            {
                _adminService.forgiveAd(adId);
                return Ok(new { message = "Successfully forgave user " + adId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        //-----
        //Bans a user
        //-----
        [HttpPost("BanUser")]
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
        [HttpPost("ForgiveUser")]
        public ActionResult ForgiveUser(int userId)
        {
            try
            {
                _adminService.forgiveUser(userId);
                return Ok(new { message = "Successfully forgave user " + userId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
