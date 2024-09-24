using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<ICollection<User>> GetAllNonAdminUsers();
        Task<User>GetUserByWorkspaceId(int workspaceId); 
        Task<bool> EmailExists(string email);

    }
}
