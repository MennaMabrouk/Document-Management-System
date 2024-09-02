using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Dto.User
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;


        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;


        [Required]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string Nid { get; set; } = string.Empty;

        [MaxLength(6)]
        public string Gender { get; set; } = string.Empty;

        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int YearOfBirth { get; set; }


    }
}
