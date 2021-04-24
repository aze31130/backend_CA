using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using System;
using System.Linq;
using static backend_CA.Utils.HashUtils;

namespace backend_CA.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User Register(User user);
        User GetUserById(int id);
    }

    public class UserService : IUserService
    {
        private Context _context;
        public UserService(Context context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            User user = _context.users.FirstOrDefault(x => x.username.Equals(username)) ?? null;

            if (user == null)
            {
                return null;
            }

            if (!AuthPassword(password, user.password, user.salt))
            {
                return null;
            }
            return user;
        }

        public User Register(User user)
        {
            if (string.IsNullOrEmpty(user.password))
            {
                throw new CustomException("You need to enter a password");
            }

            if (_context.users.Any(x => x.username.Equals(user.username)))
            {
                throw new CustomException("Username " + user.username + " is already taken");
            }

            user.salt = getRandomSalt(SALT_LENGTH);
            user.password = HashPassword(user.password, user.salt);
            user.fame = 0;
            user.lastlogin = DateTime.UtcNow;
            user.created = DateTime.UtcNow;
            user.isPremium = false;

            _context.users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User GetUserById(int id)
        {
            return _context.users.Find(id);
        }
    }
}
