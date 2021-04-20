using System;
using System.Collections.Generic;

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
        public int id {get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string email { get; set; }
        public int fame { get; set; }
        public USER_TYPE type { get; set; }
        public DateTime lastlogin { get; set; }
        public DateTime created { get; set; }
        public bool isPremium { get; set; }
    }
}
