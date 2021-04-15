namespace backend_CA.Models
{
    public enum SKILLS
    {
        POSITIVE,
        TEAM_WORKER,
        SOCIABLE,
        FAST_LEARNER,
        LEADERSHIP,
        PROBLEM_SOLVER,
        MOTIVATER
    }
    /*
     * To make sure the 2 foreign keys can be used,
     * we have to set the unsued one to -1. This value
     * will be reserved to know if the skill is reseached or
     * owned by a user.
     */
    public class Skill
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int jobId { get; set; }
        public SKILLS skill { get; set; }
    }
}
