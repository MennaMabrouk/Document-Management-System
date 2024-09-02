using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<ICollection<Document>> GetDocumentsByWorkspaceId(int userId, int workspaceId, string documentName = null, string documentType = null, string documentVersion = null);

        Task<ICollection<Document>> GetDocumentsByFolderId(int userId, int folderId,RoleEnum role);
        Task<Document> GetDocumentMetaDataByDocumentId(int userId, int documentId, RoleEnum role);
        Task<Document> GetDocumentForUpdate(int userClaims ,int documentId);
        Task<Document> GetDocumentById(int userClaims,int documentId);
        Task<Document> GetDocumentBlobById(int userClaims, int documentId, RoleEnum role);
    }
}
