using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }
    }
}   