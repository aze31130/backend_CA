using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [Authorize]
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
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Edit a Job
        //-----
        [HttpPut("edit")]
        public ActionResult<Job> EditJob(CreateJobModel model, int jobid)
        {
            Job editedjob = _context.jobs.FirstOrDefault();
            editedjob.availableSlots = 4012;
            _context.Entry(editedjob).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        //-----
        //Remove a Job
        //-----
        [HttpDelete("remove")]
        public ActionResult<Job> RemoveJob()
        {
            throw new NotImplementedException();
        }

        //-----
        //Apply to a Job
        //-----
        [HttpPost("apply")]
        public  ActionResult<Job> Apply()
        {
            throw new NotImplementedException();
        }

        //-----
        //List all Jobs
        //-----
        [HttpPost("get_all_aviable_jobs")]
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
