using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string Nid { get; set; } = string.Empty;
   
        [MaxLength(6)]
        public string Gender { get; set; } = string.Empty;

        [Range(1900,2100)]
        public int YearOfBirth { get; set; }

        [NotMapped]
        public int Age
        {

            get
            {
                var currentYear = DateTime.Now.Year;
                var age = currentYear - YearOfBirth;
                return age;
            }
        }

        public Workspace Workspace { get; set; } 


    }
}
