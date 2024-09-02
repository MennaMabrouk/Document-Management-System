using Application.Dto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFolderService
    {
        Task<bool> CreateFolder(int userClaims, FolderDto folderDto);
        Task<FolderDto> GetFolderById(int userClaims, int folderId, string roleClaims);
        Task<bool> DeleteFolder(int userClaims, int folderId);
        Task<bool> UpdateFolder(int userClaims, FolderDto folderDto);




        Task<ICollection<FolderDto>> GetAllFoldersByWorkspaceId(int userClaims, int workspaceId, string roleClaims);
        Task<ICollection<FolderDto>> GetAllFoldersByUserId(int userClaims, int userId, string roleClaims);
        Task<ICollection<FolderDto>> GetAllPublicFolders();
        Task<bool> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId);

        Task<bool> RestoreSoftDeletedFolderById(int folderId);






    }
}
