using backend_CA.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend_CA.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Chat> chats { get; set; }
        public DbSet<Message> messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(x => x.type).HasConversion(v => v.ToString(), v => (USER_TYPE)Enum.Parse(typeof(USER_TYPE), v));
            modelBuilder.Entity<Skill>().Property(x => x.skill).HasConversion(v => v.ToString(), v => (SKILLS)Enum.Parse(typeof(SKILLS), v));
        }
    }
}
