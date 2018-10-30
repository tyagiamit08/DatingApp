using System.ComponentModel.DataAnnotations;

namespace DatingApp.WebAPI.DTOs
{
    public class UserToRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You need to specify password between 4 and 8 characters.")]
        public string Password { get; set; }
    }
}
