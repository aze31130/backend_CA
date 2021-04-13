using System;
using System.Collections.Generic;

namespace backend_CA.Models
{
    public enum USER_TYPE
    {
        JOB_SEEKER,
        EMPLOYER,
        ADMINISTRATOR
    }
    public class User
    {
        public int id {get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string passwordhash { get; set; }
        public string salt { get; set; }
        public string email { get; set; }
        public DateTime lastlogin { get; set; }
        public DateTime created { get; set; }
        public USER_TYPE type { get; set; }
        public List<SKILLS> skills { get; set; }
        public bool isPremium { get; set; }
    }
}
