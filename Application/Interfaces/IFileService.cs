using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFileService
    {
        // Methods for handling workspace paths
        Task<bool> CreateWorkspacePath(string workspaceName);
        Task<bool> UpdateWorkspacePath(string oldWorkspaceName, string newWorkspaceName);

        // Methods for handling folder paths 
        Task<bool> CreateFolderPath(string workspaceName, string folderName);
        Task<bool> UpdateFolderPath(string workspaceName, string oldFolderName, string newFolderName);
        Task<bool> CreateDocument(string workspaceName, string folderName, string documentName, byte[] content);
        Task<bool> RenameDocument(string currentFilePath, string newFilePath);
        string ExtractFileName(IFormFile file, string documentName);
        string ExtractFileType(IFormFile file);
        string ExtractFileTag(IFormFile file);
        string ExtractFileVersion(IFormFile file);
        string GetAbsolutePath(params string[] paths);
        string GetRelativePath(string absolutePath);

        Task<byte[]> ReadDocumentAsBytes(string documentPath);
 

    }
}