using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WorkspaceService : BaseService, IWorkspaceService
    {
        public WorkspaceService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
           : base(unitOfWork, mapper, fileService)
        {
        }


        public async Task<WorkspaceDto> GetWorkspaceByUserId(int userClaims, int userId, string roleClaims)
        {

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            //Admin can access anyway
            if (userClaims != userId && roleClaims == "User")
                throw new UnauthorizedAccessException("You are not authorized to access this user's information.");


            var workspaceOfUser = await _unitOfWork.Workspace.GetWorkspaceByUserId(userId);
            if (workspaceOfUser == null)
                throw new KeyNotFoundException("Workspace not found for the provided user ID.");

            return _mapper.Map<WorkspaceDto>(workspaceOfUser);
        }


        public async Task<bool> UpdateWorkspace(int userClaims, WorkspaceDto workspaceDto)
        {
            if (workspaceDto == null)
                throw new ArgumentNullException(nameof(workspaceDto), "Workspace data cannot be null.");

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var workspace = await _unitOfWork.Workspace.GetWorkspaceByUserId(userClaims);
            if (workspace == null )
                throw new KeyNotFoundException("Workspace with the provided ID was not found.");

            if (workspace.UserId != userClaims || workspace.WorkspaceId != workspaceDto.WorkspaceId)
                throw new UnauthorizedAccessException("You are not authorized to update this workspace's information.");

            if (workspace.CreationDate != workspaceDto.CreationDate)
                throw new InvalidOperationException("Creation date cannot be modified.");

            if (workspace.Name != workspaceDto.Name)
            {
                var workspaceNameExists = await _unitOfWork.Workspace.EntityExists(workspaceDto.Name);
                if (workspaceNameExists)
                    throw new InvalidOperationException("This workspace name already exists!");

                var updatedPath = await _fileService.UpdateWorkspacePath(workspace.Name, workspaceDto.Name);
                if (!updatedPath)
                {
                    await _fileService.UpdateWorkspacePath(workspaceDto.Name, workspace.Name);
                    return false;
                }
                    
            }

            workspace.Name = workspaceDto.Name;
            _unitOfWork.Workspace.Update(workspace);

            var result = _unitOfWork.Save();
            return result > 0;

        }



        public async Task<WorkspaceDto> GetWorkspaceById(int userId, int workspaceId)
        {

            if (workspaceId > 0)
            {
                var workspace = await _unitOfWork.Workspace.GetById(workspaceId);

                if (workspace != null && workspace.UserId == userId)
                {
                    return _mapper.Map<WorkspaceDto>(workspace);
                }
            }

            return null;

        }
        public async Task<bool> WorkspaceExists(string WorkspaceName)
        {
            return await _unitOfWork.Workspace.EntityExists(WorkspaceName);
        }

        public async Task<bool> WorkspaceExists(int id)
        {
            return await _unitOfWork.Workspace.EntityExists(id);
        }
    }
}
