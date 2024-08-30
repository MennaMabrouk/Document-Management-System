using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Folder
    {
        [Key]
        public int FolderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; } = false;

        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }    
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
