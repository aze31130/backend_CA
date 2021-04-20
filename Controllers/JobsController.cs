using backend_CA.Models;
using System;
using System.Collections.Generic;

namespace backend_CA.Controllers
{
    public class JobsController
    {
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
