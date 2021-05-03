using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend_CA.Services
{
    public interface IJobService
    {
        void Create(CreateJobModel model, int userId);
        void Edit(CreateJobModel model, int userId, int jobId);
        void Delete(int userId, int jobId);
    }

    public class JobService : IJobService
    {
        private Context _context;
        public JobService(Context context)
        {
            _context = context;
        }

        public void Create(CreateJobModel model, int userId)
        {
            if (string.IsNullOrEmpty(model.title))
                throw new CustomException("You need to enter a title !");

            if (model.availableSlots <= 0)
                throw new CustomException("You can't create a job without available slots !");
            if (IsEmployer(userId))
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

            if (string.IsNullOrEmpty(model.title))
                throw new CustomException("You need to enter a title");

            if (string.IsNullOrEmpty(model.description))
                throw new CustomException("You need to enter a description");

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

        private bool IsEmployer(int userId)
        {
            User user = _context.users.ToList().Find(x => x.id == userId);
            return (user.type == USER_TYPE.EMPLOYER);
        }
    }
}
