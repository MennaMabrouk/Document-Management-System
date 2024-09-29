using Application.Dto;
using Application.Dto.User;
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
        Task<PaginatedResult<FolderDto>> GetPaginatedFoldersByUserId(int userClaims, int userId, string roleClaims,int pageNumber , int pageSize);
        Task<ICollection<FolderDto>> GetAllPublicFolders();

        Task<PaginatedResult<FolderDto>> GetAllPagintedPublicFolders(int pageNumber, int pageSize);

        Task<bool> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId);

        Task<bool> RestoreSoftDeletedFolderById(int folderId);






    }
}
