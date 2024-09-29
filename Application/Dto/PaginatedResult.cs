using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class PaginatedResult<T>
    {
        public ICollection<T> Items { get; set; } = new List<T>();

    }
}
