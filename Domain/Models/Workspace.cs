using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Workspace
    {
        [Key]
        public int WorkspaceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    }

}
