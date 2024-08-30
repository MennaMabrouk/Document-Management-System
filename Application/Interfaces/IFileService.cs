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

        //Methods for handling document 
        Task<bool> CreateDocument(string workspaceName, string folderName, string documentName, byte[] content);
        Task<bool> UpdateDocument(string workspaceName, string folderName, string documentName, byte[] newContent);
        string GetWorkspacePath(string workspaceName);
        string GetFolderPath(string workspaceName, string folderName);
        string GetDocumentPath(string workspaceName, string folderName, string documentName);
       

    }
}