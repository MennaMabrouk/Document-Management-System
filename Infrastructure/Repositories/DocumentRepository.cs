using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Domain.Models;

namespace Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(DataContext context) : base(context)
        {

        }

        public async Task<Document> GetDocumentMetaDataByDocumentId(int userId, int documentId, RoleEnum role)
        {
            return await _context.Documents
                .Include(d => d.Folder)
                .ThenInclude(d => d.Workspace)
                .Where(d =>
                    d.DocumentId == documentId && !d.IsDeleted &&
                    (
                        RoleEnum.Admin == role ||
                        d.Folder.Workspace.UserId == userId ||
                        d.Folder.IsPublic
                    )
                )
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<Document>> GetDocumentsByFolderId(int userId, int folderId, RoleEnum role)
        {
            return await _context.Documents
                .Include(d => d.Folder)
                .ThenInclude(d => d.Workspace)
                .Where(d =>
                    d.FolderId == folderId && !d.IsDeleted &&
                    (
                        RoleEnum.Admin == role ||
                        d.Folder.Workspace.UserId == userId ||
                        d.Folder.IsPublic
                    )
                )
                .ToListAsync();
        }

        public async Task<ICollection<Document>> GetDocumentsByWorkspaceId(int userId, int workspaceId, string documentName, string documentType, string documentVersion)
        {
            IQueryable<Document> query = _context.Documents.Include(d => d.Folder).Where(d => d.Folder.WorkspaceId == workspaceId && !d.IsDeleted);

            //Filter by owner
            /*            query = query.Where(d => d.Folder.Workspace.UserId == userId);*/

            if (!string.IsNullOrEmpty(documentName))
            {
                query = query.Where(d => d.Name.Contains(documentName));
            }

            if (!string.IsNullOrEmpty(documentType))
            {
                query = query.Where(d => d.Type == documentType);
            }

            if (!string.IsNullOrEmpty(documentVersion))
            {
                query = query.Where(d => d.Version == documentType);
            }




            return await query.ToListAsync();
        }

        public async Task<Document> GetDocumentForUpdate(int userClaims, int documentId)
        {
            return await _context.Documents
                .Include(d => d.Folder)
                .ThenInclude(d => d.Workspace)
                .Where(d =>
                    d.DocumentId == documentId && !d.IsDeleted &&
                        d.Folder.Workspace.UserId == userClaims &&
                        !d.Folder.IsDeleted
                )
                .FirstOrDefaultAsync();
        }

        public async Task<Document> GetDocumentById(int userClaims, int documentId)
        {
            var document = await _context.Documents
                  .Include(d => d.Folder)
                  .ThenInclude(d => d.Workspace)
                  .FirstOrDefaultAsync(d => d.DocumentId == documentId && d.Folder.Workspace.UserId == userClaims && !d.IsDeleted);

            if (document == null)
                return null;

            if (document.Folder.Workspace.UserId != userClaims)
                throw new UnauthorizedAccessException("You are not authorized to view this document.");

            return document;

        }

        public async Task<Document> GetDocumentBlobById(int userClaims, int documentId, RoleEnum role)
        {
            return await _context.Documents
                .Include(d => d.Folder)
                .ThenInclude(d => d.Workspace)
                .Where(d =>
                    d.DocumentId == documentId && !d.IsDeleted &&
                    !d.Folder.IsDeleted &&  
                    (
                        RoleEnum.Admin == role ||
                        d.Folder.Workspace.UserId == userClaims ||
                        d.Folder.IsPublic
                    )
                )
                .FirstOrDefaultAsync();
        }

    }
}
