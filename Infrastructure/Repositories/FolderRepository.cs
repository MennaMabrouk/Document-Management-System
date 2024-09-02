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
    public class FolderRepository : GenericRepository<Folder>, IFolderRepository
    {
        public FolderRepository(DataContext context) : base(context)
        {

        }

        public async Task<ICollection<Folder>> GetAllFoldersByUserId(int userId)
        {
            return await _context.Folders.Include(f => f.Workspace).Where(f => f.Workspace.UserId == userId && !f.IsDeleted).ToListAsync();
        }

        public async Task<ICollection<Folder>> GetAllFoldersByWorkspaceId(int workspaceId)
        {

            return await _context.Folders.Where(f => f.WorkspaceId == workspaceId && !f.IsDeleted).ToListAsync();
        }

        public async Task<ICollection<Folder>> GetAllPublicFolders()
        {
            return await _context.Folders.Where(f => f.IsPublic && !f.IsDeleted).ToListAsync();

        }

        public async Task<Folder> GetFolderIncludingWorkspaceById(int folderId)
        {
            return await _context.Folders.Include(f => f.Workspace).FirstOrDefaultAsync(f => f.FolderId == folderId && !f.IsDeleted);
        }

        public async Task<bool> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId)
        {
            var softDeletedFolders = await _context.Folders.Where(f => f.WorkspaceId == workspaceId && f.IsDeleted).ToListAsync();

            if (softDeletedFolders.Count > 0)
            {
                foreach (var folder in softDeletedFolders)
                {
                    folder.IsDeleted = false;
                }
                return true;
            }

            return false;
        }

        public async Task<bool> RestoreSoftDeletedFolderById(int folderId)
        {
            var softDeletedFolder = await _context.Folders.Where(f => f.FolderId == folderId && f.IsDeleted).FirstOrDefaultAsync();
            if (softDeletedFolder != null)
            {
                softDeletedFolder.IsDeleted = false;
                return true;
            }
            return false;
        }

    }
}

    