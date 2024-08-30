using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWorkspaceRepository : IGenericRepository<Workspace>
    {
        Task<Workspace> GetWorkspaceByUserId(int userId);
        
    }
}
