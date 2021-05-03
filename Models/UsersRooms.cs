using System.ComponentModel.DataAnnotations;

namespace backend_CA.Models
{
    public class UsersRooms
    {
        [Key]
        public int id { get; set; }
        public int roomId { get; set; }
        public int userId { get; set; }
    }
}
