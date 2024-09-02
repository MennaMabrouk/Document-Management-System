using Application.Dto.Document;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Services
{
    public class DocumentService : BaseService, IDocumentService
    {
        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
            : base(unitOfWork, mapper, fileService)
        {
        }



        public async Task<DocumentDto> GetDocumentMetaDataByDocumentId(int userClaims, int documentId, string roleClaims)
        {

            if (documentId <= 0)
                throw new ArgumentException("Invalid document ID", nameof(documentId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));


            if (!Enum.TryParse<RoleEnum>(roleClaims, true, out RoleEnum role))
            {
                throw new ArgumentException("Invalid role claim.");
            }

            var document = await _unitOfWork.Document.GetDocumentMetaDataByDocumentId(userClaims, documentId, role);
            if (document == null)
                return null;

            return _mapper.Map<DocumentDto>(document);
        }

        public async Task<ICollection<DocumentDto>> GetDocumentsByFolderId(int userClaims, int folderId, string roleClaims)
        {
            if (folderId <= 0)
                throw new ArgumentException("Invalid folder ID", nameof(folderId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));


            if (!Enum.TryParse<RoleEnum>(roleClaims, true, out RoleEnum role))
            {
                throw new ArgumentException("Invalid role claim.");
            }

            var documents = await _unitOfWork.Document.GetDocumentsByFolderId(userClaims, folderId, role);
            if (!documents.Any())
                return new List<DocumentDto>();

            return _mapper.Map<ICollection<DocumentDto>>(documents);
        }



        public async Task<ICollection<DocumentDto>> GetDocumentsByWorkspaceId(int userClaims, int workspaceId, string documentName, string documentType, string documentVersion, string roleClaims)
        {
            if (workspaceId <= 0)
                throw new ArgumentException("Invalid workspace ID", nameof(workspaceId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var workspace = await _unitOfWork.Workspace.GetById(workspaceId);
            if (workspace == null)
                throw new KeyNotFoundException("Workspace with the provided ID was not found.");

            if (workspace.UserId != userClaims && roleClaims == "User")
                throw new UnauthorizedAccessException("You are not authorized to view this workspace's documents.");

            var documents = await _unitOfWork.Document.GetDocumentsByWorkspaceId(userClaims, workspaceId, documentName, documentType, documentVersion);
            if (!documents.Any())
                return new List<DocumentDto>();

            return _mapper.Map<ICollection<DocumentDto>>(documents);

        }

        public async Task<bool> UpdateDocument(int userClaims, DocumentDto documentDto)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto), "Document data cannot be null.");

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var document = await _unitOfWork.Document.GetDocumentForUpdate(userClaims, documentDto.DocumentId);
            if (document == null)
                throw new UnauthorizedAccessException("You are not authorized to update this document.");

            if (document.CreationDate != documentDto.CreationDate)
                throw new InvalidOperationException("Creation date cannot be modified!");

            if (document.FilePath != documentDto.FilePath)
                throw new InvalidOperationException("File path cannot be modified!");

            if (document.Name != documentDto.Name)
            {

                var currentFilePath = _fileService.GetAbsolutePath(document.FilePath);
                var newFilePath = _fileService.GetAbsolutePath("Workspaces", document.Folder.Workspace.Name, document.Folder.Name, $"{documentDto.Name}{document.Type}");
                if (File.Exists(currentFilePath))
                {
                    var renameSuccess = await _fileService.RenameDocument(currentFilePath, newFilePath);
                    if (!renameSuccess)
                    {
                        throw new InvalidOperationException("Failed to update the document name or type in the file system.");
                    }

                    document.FilePath = _fileService.GetRelativePath(newFilePath);
                }
                else
                {
                    throw new FileNotFoundException("The file to be renamed does not exist at the specified path.", currentFilePath);
                }

            }

            document.Name = documentDto.Name;
            document.Version = documentDto.Version;
            document.Tag = documentDto.Tag;

            _unitOfWork.Document.Update(document);
            var result = _unitOfWork.Save();
            return result > 0;

        }

        public async Task<bool> DeleteDocument(int userClaims, int documentId)
        {
            if (documentId <= 0)
                throw new ArgumentException("Invalid document ID", nameof(documentId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var document = await _unitOfWork.Document.GetDocumentById(userClaims, documentId);
            if (document == null)
                return false;

            document.IsDeleted = true;
            _unitOfWork.Document.Update(document);
            var result = _unitOfWork.Save();

            return result > 0;


        }

        public async Task<bool> UploadDocument(int userClaims, DocumentCreateDto documentCreateDto)
        {
            if (documentCreateDto == null)
                throw new ArgumentNullException(nameof(documentCreateDto), "Document data cannot be null.");

            if (documentCreateDto.File == null || documentCreateDto.File.Length == 0)
                throw new ArgumentException("File is required.", nameof(documentCreateDto.File));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            var folder = await _unitOfWork.Folder.GetFolderIncludingWorkspaceById(documentCreateDto.FolderId);
            if (folder == null || folder.Workspace.UserId != userClaims)
                throw new UnauthorizedAccessException("You are not authorized to upload a document to this folder.");

            var fileName = _fileService.ExtractFileName(documentCreateDto.File, documentCreateDto.Name);
            var fileType = _fileService.ExtractFileType(documentCreateDto.File);
            var fileVersion = _fileService.ExtractFileVersion(documentCreateDto.File);
            var fileTag = _fileService.ExtractFileTag(documentCreateDto.File);



            using (var stream = new MemoryStream())
            {
                await documentCreateDto.File.CopyToAsync(stream);
                var success = await _fileService.CreateDocument(folder.Workspace.Name, folder.Name, $"{fileName}{fileType}", stream.ToArray());
                if (!success)
                    return false;
            }
            var absolutePath = _fileService.GetAbsolutePath(folder.Workspace.Name, folder.Name, $"{fileName}{fileType}");
            var relativeFilePath = _fileService.GetRelativePath(absolutePath);

            var documentDto = new DocumentDto
            {
                Name = fileName,
                CreationDate = DateTime.UtcNow,
                Type = fileType,
                Tag = fileTag,
                Version = fileVersion,
                FolderId = documentCreateDto.FolderId,
                FilePath = relativeFilePath

            };

            var document = _mapper.Map<Document>(documentDto);
            await _unitOfWork.Document.Create(document);
            var result = _unitOfWork.Save();

            return result > 0;

        }

        public async Task<DocumentBlobDto> GetDocumentBlobById(int userClaims, int documentId, string roleClaims)
        {
            if (documentId <= 0)
                throw new ArgumentException("Invalid document ID", nameof(documentId));

            if (userClaims <= 0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));


            if (!Enum.TryParse<RoleEnum>(roleClaims, true, out RoleEnum role))
            {
                throw new ArgumentException("Invalid role claim.");
            }

            var document = await _unitOfWork.Document.GetDocumentBlobById(userClaims, documentId, role);
            if (document == null)
                throw new UnauthorizedAccessException("Document not found");

            var relativeFilePath = document.FilePath;
            var abosluteFilePath = _fileService.GetAbsolutePath(relativeFilePath);
            var content = await _fileService.ReadDocumentAsBytes(abosluteFilePath);

            var documentBlobDto = new DocumentBlobDto
            {
                FileName = $"{document.Name}{document.Type}",
                ContentType = GetContentType(document.Type),
                Content = content

            };

            return documentBlobDto;
        }

        private string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream", // default content type
            };
        }

    }
    }
