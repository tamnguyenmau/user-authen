using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}