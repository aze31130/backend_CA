using backend_CA.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_CA.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<User> users { get; set; }
    }
}
