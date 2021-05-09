using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend_CA.Services
{
    public interface IAdminService
    {
        User GetUserById(int userId);
        void updateUser(int userId, UpdateProfileModel model);
        bool isUserIdValid(int userId);
        void banUser(int userId);
        void forgiveUser(int userId);
        void banAd(int adId);
        void forgiveAd(int adId);
        void deleteUser(int userId);
        void answerTicket(int ticketId, string answer);
        void forceUpdatePassword(int userId, string password);
    }
    
    public class AdminService : IAdminService
    {
        private Context _context;
        public AdminService(Context context)
        {
            _context = context;
        }

        //-----
        //Function to force change the password of a given user
        //-----
        public void forceUpdatePassword(int userId, string password)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new CustomException("Make sure to give a password !");
            }

            //Retrive the user from the database
            User user = _context.users.ToList().Find(x => x.id.Equals(userId));

            //Updates the password
            user.password = Utils.HashUtils.HashPassword(password, user.salt);

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to ban an ad
        //-----
        public void banAd(int adId)
        {
            if (_context.advertisements.ToList().Find(x => x.id.Equals(adId)) == null)
            {
                throw new CustomException("This ad doesn't exist !");
            }

            //Get the ad
            Advertisement ad = _context.advertisements.ToList().Find(x => x.id.Equals(adId));
            ad.isBanned = true;
            _context.Entry(ad).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to forgive an ad
        //-----
        public void forgiveAd(int adId)
        {
            if (_context.advertisements.ToList().Find(x => x.id.Equals(adId)) == null)
            {
                throw new CustomException("This ad doesn't exist !");
            }

            //Get the ad
            Advertisement ad = _context.advertisements.ToList().Find(x => x.id.Equals(adId));
            ad.isBanned = false;
            _context.Entry(ad).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to update the profile of a user
        //-----
        public void answerTicket(int ticketId, string answer)
        {
            //Check if the ticket if is valid
            if (_context.tickets.ToList().Find(x => x.id.Equals(ticketId)) == null)
            {
                throw new CustomException("This ticket doesn't exist !");
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new CustomException("Please, you must provide an answer !");
            }

            Ticket ticket = _context.tickets.ToList().Find(x => x.id.Equals(ticketId));
            ticket.answer = answer;
            ticket.isClosed = true;
            _context.Entry(ticket).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to update the profile of a user
        //-----
        public void updateUser(int userId, UpdateProfileModel model)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            if (string.IsNullOrEmpty(model.firstname) || string.IsNullOrEmpty(model.lastname)
                || string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.email))
            {
                throw new CustomException("Make sure to fill every fields !");
            }

            //Retrive the user from the database
            User user = _context.users.ToList().Find(x => x.id.Equals(userId));

            //Updates the fields
            user.firstname = model.firstname;
            user.lastname = model.lastname;
            user.username = model.username;
            user.email = model.email;
            user.isSearchingJobs = model.isSearchingJobs;

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }


        //-----
        //Function to hard delete a user
        //-----
        public void deleteUser(int userId)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Purge the user table
            User user = _context.users.ToList().Find(x => x.id.Equals(userId));
            _context.users.Remove(user);

            //Purge skills
            foreach (Skill s in _context.skills.ToList())
            {
                if (s.userId.Equals(userId))
                {
                    _context.skills.Remove(s);
                }
            }

            //Purge jobapply
            foreach (JobApply ja in _context.jobapply.ToList())
            {
                if (ja.userId.Equals(userId))
                {
                    _context.jobapply.Remove(ja);
                }
            }

            //Purge usersrooms
            foreach (UsersRooms ur in _context.usersrooms.ToList())
            {
                if (ur.userId.Equals(userId))
                {
                    _context.usersrooms.Remove(ur);
                }
            }

            //Purge messages
            foreach (Message m in _context.messages.ToList())
            {
                if (m.senderId.Equals(userId))
                {
                    _context.messages.Remove(m);
                }
            }

            //Purge rooms
            foreach (Chat c in _context.chats.ToList())
            {
                if (c.owner.Equals(userId))
                {
                    _context.chats.Remove(c);
                }
            }

            _context.SaveChanges();
        }

        //-----
        //Function to forgive a given user
        //-----
        public void forgiveUser(int userId)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Get the user
            User user = GetUserById(userId);
            user.isBanned = false;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to ban a given user
        //-----
        public void banUser(int userId)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Get the user
            User user = GetUserById(userId);
            user.isBanned = true;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
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
