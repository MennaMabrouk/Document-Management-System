using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Document
{
    public class DocumentCreateDto
    {
        [Required]
        public IFormFile File { get; set; }
        public int FolderId { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
