using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace backend_CA.Controllers
{
    [Authorize(Roles = AdminLevel.Employer + "," + AdminLevel.Moderator + "," + AdminLevel.Administrator)]
    [ApiController]
    [Route("[controller]")]
    public class EmployerController : Controller
    {
        public IConfiguration Configuration;
        private IUserService _userService;
        private readonly Context _context;
        public EmployerController(Context context, IUserService userService, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            Configuration = configuration;
        }
    }
}
