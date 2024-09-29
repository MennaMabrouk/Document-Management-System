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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {

        }

        public async Task<ICollection<User>> GetAllNonAdminUsers()
        {
            var nonAdminUsers = await (from user in _context.Users
                                       join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                       join role in _context.Roles on userRole.RoleId equals role.Id
                                       where role.Name != "Admin" //filtering 
                                       select user).ToListAsync();

            return nonAdminUsers;
        }


        public async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByWorkspaceId(int workspaceId)
        {
            return await _context.Workspaces.Where(w => w.WorkspaceId == workspaceId).Select(u => u.User).FirstOrDefaultAsync();

        }
    }
}
