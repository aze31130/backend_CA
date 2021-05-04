namespace backend_CA.Models
{
    public class UpdateProfileModel
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool isSearchingJobs { get; set; }
    }
}
