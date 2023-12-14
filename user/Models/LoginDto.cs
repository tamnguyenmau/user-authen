using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class LoginDto
    {
        [Required]
        //[EmailAddress]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}