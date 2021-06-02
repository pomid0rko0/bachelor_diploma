using System.ComponentModel.DataAnnotations;

namespace Database.Models.DTO.Requests
{
    public class UserLoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}