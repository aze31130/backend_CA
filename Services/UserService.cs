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
        User Register(int userId, RegisterModel model);
        User GetUserById(int id);
        bool isUserIdValid(int userId);
        void changeUserPassword(int userId, ChangePasswordModel model);
        void ForgotPassword(string email);
        void OpenTicket(int userId, OpenTicketModel model);
        void PostAd(int userId, AdvertisementModel model);
        void EditAd(int userId, int adId, AdvertisementModel model);
        void DeleteAd(int userId, int adId);
        JobApply Apply(int userId, int jobId);

    }

    public class UserService : IUserService
    {
        private Context _context;
        private readonly IEmailService _emailService;
        public UserService(Context context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        //-----
        //Function to post an advertisement
        //-----
        public void PostAd(int userId, AdvertisementModel model)
        {
            //Check if any fields if null
            if (string.IsNullOrEmpty(model.title) || string.IsNullOrEmpty(model.description))
            {
                throw new CustomException("Make sure to fill every fields !");
            }

            Advertisement ad = new Advertisement();
            ad.userId = userId;
            ad.title = model.title;
            ad.description = model.description;
            ad.posted = DateTime.UtcNow;
            ad.isBanned = false;

            _context.advertisements.Add(ad);
            _context.SaveChanges();
        }

        //-----
        //Function to edit an advertisement
        //-----
        public void EditAd(int userId, int adId, AdvertisementModel model)
        {
            //Check if the post exists
            if (_context.advertisements.ToList().Find(x => x.id.Equals(adId)) == null)
            {
                throw new CustomException("This post doesn't exist !");
            }
            //Check if the user has the permission to edit this post
            if (_context.advertisements.ToList().Find(x => x.id.Equals(adId) && x.userId.Equals(userId)) == null)
            {
                throw new CustomException("You cannot edit this post !");
            }

            Advertisement ad = _context.advertisements.ToList().Find(x => x.id.Equals(adId) && x.userId.Equals(userId));
            ad.title = model.title;
            ad.description = model.description;
            ad.posted = DateTime.UtcNow;
            _context.Entry(ad).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //-----
        //Function to delete an advertisement
        //-----
        public void DeleteAd(int userId, int adId)
        {
            //Check if the user has the permission to edit this post
            if (_context.advertisements.ToList().Find(x => x.id.Equals(adId) && x.userId.Equals(userId)) == null)
            {
                throw new CustomException("You cannot delete this post !");
            }
            Advertisement ad = _context.advertisements.ToList().Find(x => x.id.Equals(adId) && x.userId.Equals(userId));
            _context.advertisements.Remove(ad);
            _context.SaveChanges();
        }

        //-----
        //Function to open a ticket
        //-----
        public void OpenTicket(int userId, OpenTicketModel model)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Check if any fields if null
            if (string.IsNullOrEmpty(model.title) || string.IsNullOrEmpty(model.description))
            {
                throw new CustomException("Make sure to fill every fields !");
            }

            Ticket ticket = new Ticket();
            ticket.userId = userId;
            ticket.title = model.title;
            ticket.description = model.description;
            ticket.answer = "";
            ticket.opened = DateTime.UtcNow;
            ticket.isClosed = false;
            _context.tickets.Add(ticket);
            _context.SaveChanges();
        }

        //-----
        //Function to resend a password
        //-----
        public void ForgotPassword(string email)
        {
            if (_context.users.ToList().Find(x => x.email.Equals(email)) == null)
            {
                throw new CustomException("The email adress isn't used !");
            }

            User user = _context.users.ToList().Find(x => x.email.Equals(email));
            user.password = HashPassword(user.salt, user.salt);
            _context.SaveChanges();

            var emailAddress = new List<string>() { email };
            var emailSubject = "Password Recovery";
            var messageBody = "Your new password is:" + user.salt;

            var response = _emailService.SendEmailAsync(emailAddress, emailSubject, messageBody);

            //Placing the response to make sure the response variable has been created
            bool respond = response.IsCompletedSuccessfully;
        }

        //-----
        //Function to the password of a selfuser
        //-----
        public void changeUserPassword(int userId, ChangePasswordModel model)
        {
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            //Check if any fields if null
            if (string.IsNullOrEmpty(model.oldPassword) || string.IsNullOrEmpty(model.newPassword)
                || string.IsNullOrEmpty(model.confirmNewPassword))
            {
                throw new CustomException("Make sure to fill every fields !");
            }

            //Check if the new passwords matches
            if (!model.newPassword.Equals(model.confirmNewPassword))
            {
                throw new CustomException("The password doesn't match !");
            }

            //Check if the request is smart :)
            if (model.oldPassword.Equals(model.newPassword) && model.newPassword.Equals(model.confirmNewPassword))
            {
                throw new CustomException("Not a smart thing to do !");
            }

            //Get the hash and the salt from the database
            User user = GetUserById(userId);

            //Check if the old password is the good one
            if (!AuthPassword(model.oldPassword, user.password, user.salt))
            {
                throw new CustomException("The password is incorrect !");
            }

            user.password = HashPassword(model.confirmNewPassword, user.salt);
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
        public User Register(int userId, RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.password))
            {
                throw new CustomException("You need to enter a password");
            }

            if (_context.users.Any(x => x.username.Equals(model.username)))
            {
                throw new CustomException("Username " + model.username + " is already taken");
            }

            //Check if the request is to create an admin / mod user
            if (model.type.Equals(USER_TYPE.ADMIN) || model.type.Equals(USER_TYPE.MOD))
            {
                //If no one is logged
                if (userId < 0)
                {
                    throw new CustomException("You cannot create an admin account !");
                }

                //Get the user and check if it is an admin user
                if (!isUserIdValid(userId))
                {
                    throw new CustomException("This user id doesn't exist !");
                }

                if (!GetUserById(userId).type.Equals(USER_TYPE.ADMIN))
                {
                    throw new CustomException("Only an admin can create an admin account !");
                }
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

        public JobApply Apply(int userId, int jobId)
        {
            if (_context.users.ToList().Find(x => x.id == userId).type != USER_TYPE.JOB_SEEKER)
                throw new CustomException("you are not a jobseeker");
            Job job = _context.jobs.ToList().Find(x => x.id == jobId);
            if (job == null)
                throw new CustomException("this job doesn't exists");
            if (job.availableSlots == 0)
                throw new CustomException("this job has no available slots left");
            JobApply jobapply = new JobApply();
            jobapply.jobId = jobId;
            jobapply.userId = userId;
            jobapply.isAccepted = false;
            _context.jobapply.Add(jobapply);
            _context.SaveChanges();
            return jobapply;
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
