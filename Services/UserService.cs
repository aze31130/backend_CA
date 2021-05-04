using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static backend_CA.Utils.HashUtils;

namespace backend_CA.Services
{
    public interface IUserService
    {
        void addUserSkill(int userId, SKILLS skill);
        void removeUserSkill(int userId, SKILLS skill);
        void updateUser(int userId, UpdateProfileModel model);
        List<SKILLS> GetUserSkills(int userId);
        List<SKILLS> getRequiredSkills(int jobId);
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
        //Function to add a skill to the skillSet of a user
        //-----
        public void addUserSkill(int userId, SKILLS skill)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }
            Skill s = new Skill();
            //-1 because this is a user skill
            s.jobId = -1;
            s.userId = userId;
            s.skill = skill;
            _context.skills.Add(s);
            _context.SaveChanges();
        }


        //-----
        //Returns the list of all required skills for a given user
        //-----
        public List<SKILLS> GetUserSkills(int userId)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("The given user doesn't exist !");
            }
            List<SKILLS> userSkills = new List<SKILLS> { };

            foreach (Skill s in _context.skills.ToList().FindAll(x => x.userId.Equals(userId)))
            {
                userSkills.Add(s.skill);
            }

            return userSkills;
        }

        //-----
        //Function to remove a skill to the skillSet of a user
        //-----
        public void removeUserSkill(int userId, SKILLS skill)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("The user id is invalid !");
            }

            //Find and remove the skill
            foreach (Skill s in _context.skills.ToList().FindAll(x => x.userId.Equals(userId)))
            {
                Console.WriteLine(s.id);
                if (s.skill.Equals(skill))
                {
                    _context.skills.Remove(s);
                }
            }
            _context.SaveChanges();
        }

        //-----
        //Returns the list of all required skills for a given job
        //-----
        public List<SKILLS> getRequiredSkills(int jobId)
        {
            throw new NotImplementedException();
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
            user.fame = 0;
            user.type = model.type;
            user.lastlogin = DateTime.UtcNow;
            user.created = DateTime.UtcNow;
            user.isPremium = false;
            user.isSearchingJobs = false;
            user.isBanned = false;
            _context.users.Add(user);
            _context.SaveChanges();
            return user;
        }

        //-----
        //Returns the list of every skill of a given user
        //-----
        public List<Skill> userSkills(int userId)
        {
            return _context.skills.ToList().FindAll(x => x.userId.Equals(userId));
        }

        //-----
        //Returns the list of every skill of a given user
        //-----
        public Skill findSkill(int userId, SKILLS skill)
        {
            return userSkills(userId).Find(x => x.skill.Equals(skill));
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
