using backend_CA.Data;
using backend_CA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    public class JobsController : Controller
    {
        private readonly Context _context;
        public JobsController(Context context)
        {
            _context = context;
        }

        //-----
        //Create a Job
        //-----
        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob()
        {
            throw new NotImplementedException();
        }

        //-----
        //Edit a Job
        //-----
        [HttpPut]
        public async Task<ActionResult<Job>> EditJob()
        {
            throw new NotImplementedException();
        }

        //-----
        //Remove a Job
        //-----
        [HttpDelete]
        public async Task<ActionResult<Job>> RemoveJob()
        {
            throw new NotImplementedException();
        }

        //-----
        //Apply to a Job
        //-----
        [HttpPost]
        public async Task<ActionResult<Job>> Apply()
        {
            throw new NotImplementedException();
        }

        //-----
        //List all Jobs
        //-----
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Job>>> GetAllAvailableJobs()
        {
            throw new NotImplementedException();
        }

        //Function to apply to a job
        public void apply(User user, int jobId)
        {
            throw new NotImplementedException();
        }

        //Function to get the job history of a user
        public void getJobHistory(int id)
        {
            throw new NotImplementedException();
        }

        private int computeScore(int days, int fame, bool isPremium)
        {
            if (isPremium)
            {
                fame += days/2;
            }
            return (int)(2 * (fame ^ 3)) + days;
        }

        //Function to search available jobs
        public void getJob(User user, List<Skill> researshedSKills)
        {
            //First, get all the user skills
            //Get if the user is premium
            //get all jobs and filter the researshedSkills
            //Apply if the member is premium

            throw new NotImplementedException();
        }
    }
}
