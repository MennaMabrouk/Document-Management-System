using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FolderService : BaseService, IFolderService
    {
        public FolderService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
            : base(unitOfWork, mapper, fileService)
        {
        }

        public async Task<bool> CreateFolder(int userClaims, FolderDto folderDto)
        {
            if (folderDto == null)
                throw new ArgumentNullException(nameof(folderDto), "Folder data cannot be null.");

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var workspace = await _unitOfWork.Workspace.GetWorkspaceByUserId(userClaims);
            if (workspace == null)
                throw new InvalidOperationException("Workspace is not found for the logged-in user.");

            var FolderPath = await _fileService.CreateFolderPath(workspace.Name, folderDto.Name);
            if (!FolderPath)
            {
                return false;
            }

            var folder = _mapper.Map<Folder>(folderDto);
            folder.CreationDate = System.DateTime.UtcNow;
            folder.WorkspaceId = workspace.WorkspaceId;
            folder.IsDeleted = false;


            await _unitOfWork.Folder.Create(folder);
            var result = _unitOfWork.Save();


            if (result > 0)
            {
                folderDto.FolderId = folder.FolderId;
                return true;
            }
            return false;
        }



        public async Task<bool> DeleteFolder(int userClaims, int folderId)
        {
            if (folderId <= 0)
                throw new ArgumentException("Invalid folder ID", nameof(folderId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var folderToDelete = await _unitOfWork.Folder.GetFolderIncludingWorkspaceById(folderId);
            if (folderToDelete == null)
                throw new KeyNotFoundException("Folder with the provided ID was not found.");

            if (folderToDelete.Workspace.UserId != userClaims)
                throw new UnauthorizedAccessException("You are not authorized to delete this folder.");

            folderToDelete.IsDeleted = true;
            _unitOfWork.Folder.Update(folderToDelete);
            var result = _unitOfWork.Save();

            return result > 0;


        }


        public async Task<ICollection<FolderDto>> GetAllFoldersByWorkspaceId(int userClaims, int workspaceId, string roleClaims)
        {
            if (workspaceId <= 0)
                throw new ArgumentException("Invalid workspace ID", nameof(workspaceId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var workspace = await _unitOfWork.Workspace.GetById(workspaceId);
            if (workspace == null)
                throw new KeyNotFoundException("Workspace not found for the provided ID.");

            //Admin can access anyway
            if (userClaims != workspace.UserId && roleClaims == "User")
                throw new UnauthorizedAccessException("You are not authorized to access this folder's information.");

            var folders = await _unitOfWork.Folder.GetAllFoldersByWorkspaceId(workspaceId);
            if (!folders.Any())
                return new List<FolderDto>();

            return _mapper.Map<ICollection<FolderDto>>(folders);
        }



        public async Task<ICollection<FolderDto>> GetAllPublicFolders(int userClaims)
        {
            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var publicFolders = await _unitOfWork.Folder.GetAllPublicFolders(userClaims);
            if (!publicFolders.Any())
                return new List<FolderDto>();

            return _mapper.Map<ICollection<FolderDto>>(publicFolders);
        }

        public async Task<FolderDto> GetFolderById(int userClaims, int folderId, string roleClaims)
        {
            if (folderId <= 0)
                throw new ArgumentException("Invalid folder ID", nameof(folderId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var folder = await _unitOfWork.Folder.GetFolderIncludingWorkspaceById(folderId);
            if (folder == null)
                throw new KeyNotFoundException("Folder not found for the provided ID.");

            //Admin can access anyway
            if (userClaims != folder.Workspace.UserId && roleClaims == "User")
                throw new UnauthorizedAccessException("You are not authorized to access this folder's information.");


            return _mapper.Map<FolderDto>(folder);

        }



        public async Task<bool> UpdateFolder(int userClaims, FolderDto folderDto)
        {
            if (folderDto == null)
                throw new ArgumentNullException(nameof(folderDto), "Folder data cannot be null.");

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var folder = await _unitOfWork.Folder.GetFolderIncludingWorkspaceById(folderDto.FolderId);
            if (folder == null)
                throw new KeyNotFoundException("Folder with the provided ID was not found.");

            if (folder.Workspace.UserId != userClaims)
                throw new UnauthorizedAccessException("You are not authorized to update this folder's information.");

            if (folder.CreationDate != folderDto.CreationDate)
                throw new InvalidOperationException("Creation date cannot be modified.");


            var updatedPath = await _fileService.UpdateFolderPath(folder.Workspace.Name, folder.Name, folderDto.Name);
            if (!updatedPath)
            {
                await _fileService.UpdateFolderPath(folder.Workspace.Name, folderDto.Name, folder.Name);
                return false;
            }


            folder.Name = folderDto.Name;
            folder.IsPublic = folderDto.IsPublic;
            _unitOfWork.Folder.Update(folder);

            var result = _unitOfWork.Save();
            return result > 0;

        }

        public async Task<bool> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId)
        {
            var result = await _unitOfWork.Folder.RestoreAllSoftDeletedFoldersByWorkspaceId(workspaceId);
            _unitOfWork.Save();

            return result;

        }

        public async Task<bool> RestoreSoftDeletedFolderById(int folderId)
        {
            var result = await _unitOfWork.Folder.RestoreSoftDeletedFolderById(folderId);
            _unitOfWork.Save();
            return result;
        }


        public async Task<bool> FolderExists(int id)
        {
            return await _unitOfWork.Folder.EntityExists(id);
        }

        public async Task<bool> FolderExists(string name)
        {
            return await _unitOfWork.Folder.EntityExists(name);
        }

    }
}
