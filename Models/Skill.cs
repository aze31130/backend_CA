using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public enum SKILLS : int
    {
        POSITIVE = 1,
        TEAM_WORKER = 2,
        SOCIABLE = 3,
        FAST_LEARNER = 4,
        LEADERSHIP = 5,
        PROBLEM_SOLVER = 6,
        MOTIVATER = 7
    }
    /*
     * To make sure the 2 foreign keys can be used,
     * we have to set the unsued one to -1. This value
     * will be reserved to know if the skill is reseached or
     * owned by a user.
     */
    public class Skill
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public int jobId { get; set; }
        public SKILLS skill { get; set; }
    }
}
