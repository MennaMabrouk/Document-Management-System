using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WorkspaceRepository : GenericRepository<Workspace> , IWorkspaceRepository
    {
        public WorkspaceRepository(DataContext context) : base(context)
        {
        }

        public async Task<Workspace> GetWorkspaceByUserId(int userId)
        {
            return await _context.Workspaces.Where(w => w.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
