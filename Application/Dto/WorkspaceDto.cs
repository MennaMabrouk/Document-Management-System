
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class WorkspaceDto
    {
        public int WorkspaceId { get; set; }
   
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }


    }
}
