namespace backend_CA.Models
{
    public class CreateJobModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public int availableSlots { get; set; }
    }
}
