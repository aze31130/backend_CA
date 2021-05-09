using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace backend_CA.Services
{
    public interface IJobService
    {
        void Create(CreateJobModel model, int userId);
        void Edit(CreateJobModel model, int userId, int jobId);
        void Delete(int userId, int jobId);
        void Choose(int userId, int jobRequestId);
        List<JobRequestModel> GetRequests(int userId, int jobId);
        List<SKILLS> GetUserSkills(int userId);
    }

    public class JobService : IJobService
    {
        private Context _context;
        public JobService(Context context)
        {
            _context = context;
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

        public List<JobRequestModel> GetRequests(int userId, int jobId)
        {
            //Check if the given user is valid
            if (!isUserIdValid(userId))
            {
                throw new CustomException("This user doesn't exist !");
            }

            if (_context.jobs.ToList().Find(x => x.id.Equals(jobId) && x.employerId.Equals(userId)) == null)
            {
                throw new CustomException("The selected job doesn't exist !");
            }

            List<JobRequestModel> JobRequestUsers = new List<JobRequestModel> { };

            foreach (JobApply ja in _context.jobapply.ToList())
            {
                if (ja.jobId.Equals(jobId))
                {

                    User u = GetUserById(ja.userId);
                    JobRequestModel jrm = new JobRequestModel();
                    jrm.userId = u.id;
                    jrm.userFirstname = u.firstname;
                    jrm.userLastname = u.lastname;
                    jrm.userEmail = u.email;
                    jrm.userSkills = GetUserSkills(u.id);

                    JobRequestUsers.Add(jrm);
                }
            }
            return JobRequestUsers;
        }

        public void Create(CreateJobModel model, int userId)
        {
            if (string.IsNullOrEmpty(model.title) && string.IsNullOrEmpty(model.description))
                throw new CustomException("You need to enter a title and a description !");

            if (model.availableSlots <= 0)
                throw new CustomException("You can't create a job without available slots !");
            if (!IsEmployer(userId))
                throw new CustomException("You need to be a employer in order to create a job request !");

            Job job = new Job();
            job.title = model.title;
            job.description = model.description;
            job.availableSlots = model.availableSlots;
            job.employerId = userId;

            _context.jobs.Add(job);
            _context.SaveChanges();
        }

        public void Edit(CreateJobModel model, int userId, int jobId)
        {
            Job editedjob = _context.jobs.ToList().Find(x => x.id == jobId);
            if (editedjob == null)
                throw new CustomException("invalid Id");

            if (!(IsEmployer(userId) && userId == editedjob.employerId))
                throw new CustomException("you can't edit this job");

            if (string.IsNullOrEmpty(model.title) && string.IsNullOrEmpty(model.description))
                throw new CustomException("You need to enter a title and a description");

            if (model.availableSlots <= 0)
                throw new CustomException("your can't edit a job without aviable slots.");

            editedjob.title = model.title;
            editedjob.description = model.description;
            editedjob.availableSlots = model.availableSlots;

            _context.Entry(editedjob).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int userId, int jobId)
        {
            Job removedjob = _context.jobs.ToList().Find(x => x.id == jobId);
            if (removedjob == null)
                throw new CustomException("invalid Id !");

            if (!(IsEmployer(userId) && userId == removedjob.employerId))
                throw new CustomException("you can't delete this job");

            _context.jobs.Remove(removedjob);
            _context.SaveChanges();
        }

        public void Choose(int userId, int jobRequestId)
        {
            Job j = _context.jobs.ToList().Find(x => x.id.Equals(jobRequestId));
            
            //Check if the jobRequest exists
            if (j == null)
            {
                throw new CustomException("This job request doesn't exist !");
            }

            //Check if the employer owns the job request
            if (j.employerId != userId)
            {
                throw new CustomException("You do not own this job request !");
            }

            //Decrease the available slots
            j.availableSlots--;

            if (j.availableSlots <= 0)
            {
                //Delete the entry
                _context.jobs.Remove(j);

                //Delete every job request with this id
                foreach (JobApply ja in _context.jobapply.ToList())
                {
                    if (ja.jobId.Equals(jobRequestId))
                    {
                        _context.jobapply.Remove(ja);
                    }
                }
            }
            else
            {
                //Get the request and set isAccepted to true
                JobApply ja = _context.jobapply.ToList().Find(x => x.jobId.Equals(jobRequestId));
                ja.isAccepted = true;
                _context.Entry(ja).State = EntityState.Modified;
            }

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

        //Check if the given user id is employer or mod or admin
        private bool IsEmployer(int userId)
        {
            return (_context.users.ToList().Find(x => x.id.Equals(userId)).type.Equals(USER_TYPE.EMPLOYER));
        }
    }
}
