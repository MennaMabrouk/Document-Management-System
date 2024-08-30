using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWorkspaceService
    {
        Task<WorkspaceDto> GetWorkspaceByUserId(int userClaims,int userId,string roleClaims);
        Task<WorkspaceDto> GetWorkspaceById(int userId ,int workspaceId);
        Task<bool> UpdateWorkspace(int userClaims, WorkspaceDto workspaceDto);
        Task<bool> WorkspaceExists(int id);
        Task<bool> WorkspaceExists(string WorkspaceName);
    }
}
