using Application.Dto.Document;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDocumentService
    {
        Task<ICollection<DocumentDto>> GetDocumentsByWorkspaceId(int userClaims, int workspaceId, string documentName, string documentType, string documentVersion, string roleClaims);
        Task<ICollection<DocumentDto>> GetDocumentsByFolderId(int userClaims, int folderId,string roleClaims);
        Task<DocumentDto> GetDocumentMetaDataByDocumentId(int userClaims,int documentId , string roleClaims);
        Task<bool> UpdateDocument(int userClaims, DocumentDto documentDto);
        Task<bool> DeleteDocument(int userClaims, int documentId);
        Task<bool> UploadDocument(int userClaims, DocumentCreateDto documentCreateDto);
        Task<DocumentBlobDto> GetDocumentBlobById(int userClaims, int documentId, string roleClaims);

    }
}
