using System.ComponentModel.DataAnnotations;

namespace BestStore.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="First Name required"), MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last Name required"), MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage ="phone format is invalid"), MaxLength(10)]
        public string? PhoneNumber { get; set; }

        [Required,  MaxLength(200)]
        public string? Address { get; set; } = "";

        [Required, MaxLength(200)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage ="confirm password is required")]
        [Compare("Password", ErrorMessage = "password is not matched ")]
        public string ConfirmedPassword { get; set; } = "";
    }
}
