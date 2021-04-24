using System;
using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public enum USER_TYPE : int
    {
        JOB_SEEKER = 1,
        EMPLOYER = 2,
        ADMIN = 3
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
        public int adminLevel { get; set; }
        public int fame { get; set; }
        public USER_TYPE type { get; set; }
        public DateTime lastlogin { get; set; }
        public DateTime created { get; set; }
        public bool isPremium { get; set; }
    }
}
