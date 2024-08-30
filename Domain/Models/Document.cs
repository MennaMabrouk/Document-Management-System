
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Version { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Tag { get; set; } = string.Empty;
        [MaxLength(20)]
        public string AccessControl { get; set; } = "Private";

        public int FolderId { get; set; }
        public Folder Folder { get; set; } 

    }
}
