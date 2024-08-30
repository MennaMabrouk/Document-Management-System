using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFolderRepository : IGenericRepository<Folder>
    {
        Task<ICollection<Folder>> GetAllFoldersByWorkspaceId(int workspaceId);
        Task<Folder> GetFolderIncludingWorkspaceById(int folderId);
        Task<ICollection<Folder>> GetAllPublicFolders(int userClaims);

        Task<bool> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId);

        Task<bool> RestoreSoftDeletedFolderById(int folderId);

    }
}
