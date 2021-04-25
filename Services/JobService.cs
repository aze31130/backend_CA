using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Utils;

namespace backend_CA.Services
{
    public interface IJobService
    {
        Job Create(CreateJobModel model, int userId);
    }

    public class JobService : IJobService
    {
        private Context _context;
        public JobService(Context context)
        {
            _context = context;
        }

        public Job Create(CreateJobModel model, int userId)
        {
            if (string.IsNullOrEmpty(model.title))
                throw new CustomException("You need to enter a title");

            if (model.aviableSlots <= 0)
                throw new CustomException("your can't create a job without aviable slots.");

            Job job = new Job();
            job.title = model.title;
            job.description = model.description;
            job.availableSlots = model.aviableSlots;
            job.employerId = userId;

            return job;
        }
    }
}
