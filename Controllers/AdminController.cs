using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
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
        private IUserService _userService;
        private readonly Context _context;
        public AdminController(Context context, IUserService userService, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            Configuration = configuration;
        }


        [HttpDelete("id")]
        public ActionResult<User> DeleteUser(int id)
        {
            /*
            var user = _context.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.users.Remove(user);
                _context.SaveChanges();
                return user;
            }
            */
            return null;
        }


    }
}
