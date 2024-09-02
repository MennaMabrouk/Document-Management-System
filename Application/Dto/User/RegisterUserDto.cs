using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.User
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]

        public string Nid { get; set; }

        [MaxLength(6)]
        public string Gender { get; set; }

        [Required]
        public string WorkspaceName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
        [Range(1900, 2100)]
        public int YearOfBirth { get; set; }

    }
}
