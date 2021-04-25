using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class RegisterModel
    {
        [Required]
        public USER_TYPE type { get; set; }
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastname { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
