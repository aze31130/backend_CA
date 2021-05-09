using System.Collections.Generic;

namespace backend_CA.Models
{
    public class JobRequestModel
    {
        public int userId { get; set; }
        public string userFirstname { get; set; }
        public string userLastname { get; set; }
        public string userEmail { get; set; }
        public List<SKILLS> userSkills { get; set; }
    }
}
