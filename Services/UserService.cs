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
        User Register(RegisterModel model);
        User GetUserById(int id);
        bool isUserIdValid(int userId);
    }

    public class UserService : IUserService
    {
        private Context _context;
        public UserService(Context context)
        {
            _context = context;
        }

        //-----
        //Returns the user object if the given credentials are correct
        //-----
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

            if (user.isBanned)
            {
                throw new CustomException("This user has been banned !");
            }

            if (!AuthPassword(password, user.password, user.salt))
            {
                return null;
            }
            return user;
        }

        //-----
        //Registers a new user in the dabase
        //-----
        public User Register(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.password))
            {
                throw new CustomException("You need to enter a password");
            }

            if (_context.users.Any(x => x.username.Equals(model.username)))
            {
                throw new CustomException("Username " + model.username + " is already taken");
            }

            //Create the user object
            User user = new User();
            user.firstname = model.firstname;
            user.lastname = model.lastname;
            user.username = model.username;
            user.salt = getRandomSalt(SALT_LENGTH);
            user.password = HashPassword(model.password, user.salt);
            user.email = model.email;
            user.adminLevel = 0;
            user.fame = 0;
            user.type = model.type;
            user.lastlogin = DateTime.UtcNow;
            user.created = DateTime.UtcNow;
            user.isPremium = false;
            user.isBanned = false;
            _context.users.Add(user);
            _context.SaveChanges();
            return user;
        }

        //-----
        //Returns the user object for a given userId
        //-----
        public User GetUserById(int userId)
        {
            return _context.users.Find(userId);
        }

        //-----
        //Returns true if the user id is valid
        //-----
        public bool isUserIdValid(int userId)
        {
            if (GetUserById(userId) == null)
            {
                return false;
            }
            return true;
        }
    }
}
