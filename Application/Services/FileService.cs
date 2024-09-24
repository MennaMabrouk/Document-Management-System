using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;

    public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<bool> CreateWorkspacePath(string workspaceName)
    {
        string workspacePath = GetAbsolutePath("Workspaces",workspaceName);
        return await CreateDirectoryAsync(workspacePath);
    }

    public async Task<bool> UpdateWorkspacePath(string oldWorkspaceName, string newWorkspaceName)
    {
        string oldFolderPath = GetAbsolutePath("Workspaces",oldWorkspaceName);
        string newFolderPath = GetAbsolutePath("Workspaces",newWorkspaceName);
        return await MoveDirectoryAsync(oldFolderPath, newFolderPath);
    }

    public async Task<bool> CreateFolderPath(string workspaceName, string folderName)
    {
        string folderPath = GetAbsolutePath("Workspaces", workspaceName, folderName);
        return await CreateDirectoryAsync(folderPath);
    }

    public async Task<bool> UpdateFolderPath(string workspaceName, string oldFolderName, string newFolderName)
    {
        string oldFolderPath = GetAbsolutePath("Workspaces",workspaceName, oldFolderName);
        string newFolderPath = GetAbsolutePath("Workspaces", workspaceName, newFolderName);
        if (Directory.Exists(newFolderPath))
        {
            throw new IOException($"The destination folder already exists: {newFolderPath}");
        }

        return await MoveDirectoryAsync(oldFolderPath, newFolderPath);
    }


    private async Task<bool> CreateDirectoryAsync(string path)
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Directory.Exists(path);
        });
    }

    private async Task<bool> MoveDirectoryAsync(string oldPath, string newPath)
    {
        try
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(oldPath))
                {
                    if (!Directory.Exists(newPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    else
                    {
                        throw new IOException($"The destination directory already exists: {newPath}");
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException($"The source directory does not exist: {oldPath}");
                }

                return Directory.Exists(newPath);
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error moving directory: {ex.Message}");
            return false;
        }
    }


    public string ExtractFileName(IFormFile file, string documentName)
    {
        return string.IsNullOrEmpty(documentName)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : documentName;
    }

    public string ExtractFileType(IFormFile file)
    {
        var fileType =  Path.GetExtension(file.FileName);
        if(!fileType.StartsWith("."))
        {
            fileType = $".{fileType}";
        }

        return fileType;    
    }

    public string ExtractFileTag(IFormFile file)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

        // Example: Extract the part of the name after the first underscore
        var parts = fileNameWithoutExtension.Split('_');

        // Assuming the second part is the tag
        return parts.Length > 1 ? parts[1] : "General";
    }

    public string ExtractFileVersion(IFormFile file)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

        // Assuming the version is after the last underscore (e.g., "document_v2.0")
        var parts = fileNameWithoutExtension.Split('_');
        var versionPart = parts.LastOrDefault();

        // Check if the last part starts with 'v' and is followed by numbers
        if (!string.IsNullOrEmpty(versionPart) && versionPart.StartsWith("v"))
        {
            return versionPart.Substring(1); // Return the version part without the 'v'
        }

        return "1.0"; // Default version if not found
    }


    public string GetAbsolutePath(params string[] paths)
    {
        return Path.Combine(_environment.WebRootPath, Path.Combine(paths));
    }

    public string GetRelativePath(string absolutePath)
    {
        return absolutePath.Replace(_environment.WebRootPath, "").TrimStart(Path.DirectorySeparatorChar);
    }

    public async Task<bool> CreateDocument(string workspaceName, string folderName, string documentName, byte[] content)
    {
        string documentPath = GetAbsolutePath("Workspaces", workspaceName, folderName, documentName);
        if (!File.Exists(documentPath))
        {
            await File.WriteAllBytesAsync(documentPath, content);
        }
        return File.Exists(documentPath);
    }

    public async Task<bool> RenameDocument(string currentFilePath, string newFilePath)
    {

            if (!File.Exists(currentFilePath))
            {
                throw new FileNotFoundException("The file to be renamed does not exist.");
            }
            File.Move(currentFilePath, newFilePath);
            return await Task.FromResult(File.Exists(newFilePath));

        
    }

    public async Task<byte[]> ReadDocumentAsBytes(string documentPath)
    {
 
        if (!File.Exists(documentPath))
        {
            throw new FileNotFoundException("The document does not exist at the specified path.");
        }
        return await File.ReadAllBytesAsync(documentPath);
    }
}

