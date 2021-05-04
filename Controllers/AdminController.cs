using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
