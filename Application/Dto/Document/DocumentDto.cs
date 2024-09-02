
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Dto.Document
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Version { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Tag { get; set; } = string.Empty;

        public int FolderId { get; set; }


    }
}
