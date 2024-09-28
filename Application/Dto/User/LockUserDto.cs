using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.User
{
    public class LockUserDto
    {
        public int LockTime { get; set; }
        public string TimeUnit { get; set; }
    }
}
