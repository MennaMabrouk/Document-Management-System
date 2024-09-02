using Application.Dto.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int userClaims, int userId, string roleClaims);
        Task<bool> UpdateUser(int userClaims, UserDto userDto);
        Task<UserDto> GetUserByWorkspaceId(int workspaceId);
        Task<bool> CreateWorkspacePathForUser(string workspaceName);

    }

}