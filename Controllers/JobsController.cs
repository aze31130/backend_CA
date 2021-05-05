using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend_CA.Controllers
{
    [Authorize(Roles = AdminLevel.Employer + "," + AdminLevel.Moderator + "," + AdminLevel.Administrator)]
    [ApiController]
    [Route("[controller]")]
    public class JobsController : Controller
    {
        private readonly Context _context;
        private IJobService _jobService;

        public JobsController(Context context, IJobService jobService)
        {
            _context = context;
            _jobService = jobService;
        }

        //-----
        //Create a Job
        //-----
        [HttpPost("create")]
        public  ActionResult<Job> CreateJob(CreateJobModel model)
        {
            int userId = GetUserId(); 
            try
            {
                _jobService.Create(model, userId);
                return Ok(new { message = "Job offer successfully created" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Edit a Job
        //-----
        [HttpPut("edit")]
        public ActionResult<Job> EditJob(CreateJobModel model, int jobId)
        {
            int userId = GetUserId();
            try
            {
                _jobService.Edit(model, jobId, userId);
                return Ok(new { message = "Job successfully edited" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Remove a Job
        //-----
        [HttpDelete("remove")]
        public ActionResult<Job> RemoveJob(int jobId)
        {
            int userId = GetUserId();
            try
            {
                _jobService.Delete(userId, jobId);
                return Ok(new { message = "Job successfully deleted" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //Function to choose a given 
        //-----
        [HttpPost("Choose")]
        public ActionResult<Job> Choose(int jobApplyId)
        {
            try
            {
                _jobService.Choose(GetUserId(), jobApplyId);
                return Ok(new { message = "Successfully choosed job: " + jobApplyId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        //-----
        //List all Jobs
        //-----
        [HttpPost("get_all_available_jobs")]
        public  ActionResult<IEnumerable<Job>> GetAllAvailableJobs()
        {
            List<Job> Jobslist = _context.jobs.ToList();
            List<Job> aviablejobs = new List<Job> { };
            foreach (Job currentjob in Jobslist)
            {
                if (currentjob.availableSlots != 0)
                    aviablejobs.Add(currentjob);
            }
            return aviablejobs;
        }

        private int GetUserId()
        {
            return int.Parse(User.Identity.Name);
        }
    }
}
