using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class ChangePasswordDto
    {
       //ChangePasswordDtonotdone
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}