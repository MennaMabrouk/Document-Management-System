using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<bool> CreateWorkspacePath(string workspaceName)
        {
            string folderPath = GetWorkspacePath(workspaceName);
            return await CreateDirectoryAsync(folderPath);
        }

        public async Task<bool> UpdateWorkspacePath(string oldWorkspaceName, string newWorkspaceName)
        {
            string oldFolderPath = GetWorkspacePath(oldWorkspaceName);
            string newFolderPath = GetWorkspacePath(newWorkspaceName);
            return await MoveDirectoryAsync(oldFolderPath, newFolderPath);
        }

        public async Task<bool> CreateFolderPath(string workspaceName, string folderName)
        {
            string folderPath = GetFolderPath(workspaceName, folderName);
            return await CreateDirectoryAsync(folderPath);
        }

        public async Task<bool> UpdateFolderPath(string workspaceName, string oldFolderName, string newFolderName)
        {
            string oldFolderPath = GetFolderPath(workspaceName, oldFolderName);
            string newFolderPath = GetFolderPath(workspaceName, newFolderName);
            return await MoveDirectoryAsync(oldFolderPath, newFolderPath);
        }

        public async Task<bool> CreateDocument(string workspaceName, string folderName, string documentName, byte[] content)
        {
            string documentPath = GetDocumentPath(workspaceName, folderName, documentName);

            return await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(documentPath))
                    {
                        File.WriteAllBytes(documentPath, content);
                    }
                    return File.Exists(documentPath);
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public async Task<bool> UpdateDocument(string workspaceName, string folderName, string documentName, byte[] newContent)
        {
            string documentPath = GetDocumentPath(workspaceName, folderName, documentName);

            return await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(documentPath))
                    {
                        File.WriteAllBytes(documentPath, newContent);
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public string GetWorkspacePath(string workspaceName)
        {
            return Path.Combine(_environment.WebRootPath, "Workspaces", workspaceName);
        }

        public string GetFolderPath(string workspaceName, string folderName)
        {
            return Path.Combine(GetWorkspacePath(workspaceName), folderName);
        }

        public string GetDocumentPath(string workspaceName, string folderName, string documentName)
        {
            return Path.Combine(GetFolderPath(workspaceName, folderName), documentName);
        }

        private async Task<bool> CreateDirectoryAsync(string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    return Directory.Exists(path);
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private async Task<bool> MoveDirectoryAsync(string oldPath, string newPath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    return Directory.Exists(newPath);
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
