using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
