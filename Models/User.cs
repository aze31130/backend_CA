using System;
using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    /*
     * The MOD user can ban (soft delete) anything 
     * The ADMIN user is able to hard delete anything
     */
    public enum USER_TYPE : int
    {
        JOB_SEEKER = 1,
        EMPLOYER = 2,
        MOD = 3,
        ADMIN = 4
    }

    public class User
    {
        [Key]
        public int id {get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string email { get; set; }
        public int fame { get; set; }
        public USER_TYPE type { get; set; }
        public DateTime lastlogin { get; set; }
        public DateTime created { get; set; }
        public bool isPremium { get; set; }
        public bool isSearchingJobs { get; set; }
        public bool isBanned { get; set; }
    }
}
