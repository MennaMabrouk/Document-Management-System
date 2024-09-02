using Application.Dto.Document;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DocumentController  : BaseController
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService) 
        {
            _documentService = documentService;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("Workspace/{workspaceId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<DocumentDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDocumentsByWorkspaceId(int workspaceId, [FromQuery] string documentName = null, [FromQuery] string documentType = null, [FromQuery] string documentVersion = null)
        
        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
           var documents = await _documentService.GetDocumentsByWorkspaceId(userIdClaims,workspaceId, documentName, documentType, documentVersion, roles);
            if(!documents.Any())
                return NoContent();
            
            return Ok(documents);
        }



        [Authorize(Roles = "User,Admin")]
        [HttpGet("Folder/{folderId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<DocumentDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDocumentsByFolderId(int folderId)

        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var documents = await _documentService.GetDocumentsByFolderId(userIdClaims, folderId, roles);
            if (!documents.Any())
                return NoContent();

            return Ok(documents);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpGet("{documentId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<DocumentDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDocumentMetaDataByDocumentId(int documentId)

        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var document = await _documentService.GetDocumentMetaDataByDocumentId(userIdClaims, documentId, roles);
            if (document == null)
                return NoContent();

            return Ok(document);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("Preview/{documentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PreviewDocument(int documentId)

        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var documentBlob = await _documentService.GetDocumentBlobById(userIdClaims, documentId, roles);
            if (documentBlob == null)
                return NotFound("Document not found!");

            Response.Headers.Add("Content-Disposition", $"inline; filename={documentBlob.FileName}");
            return File(documentBlob.Content, documentBlob.ContentType);
        }



        [Authorize(Roles = "User,Admin")]
        [HttpGet("Download/{documentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DownloadDocument(int documentId)

        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var documentBlob = await _documentService.GetDocumentBlobById(userIdClaims, documentId, roles);
            if (documentBlob == null)
                return NotFound("Document not found!");

            return File(documentBlob.Content, documentBlob.ContentType, documentBlob.FileName);
        }

        [Authorize(Roles = "User")]
        [HttpPost("Upload")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> UploadDocument([FromForm] DocumentCreateDto documentCreateDto)
        {

            var userIdClaims = GetUserIdFromClaims();
            var updateResult = await _documentService.UploadDocument(userIdClaims, documentCreateDto);
            if (!updateResult)
                return BadRequest("Failed to update!");


            return StatusCode(201, "Document uploaded successfully.");

        }


        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> UpdateDocument([FromBody] DocumentDto updatedDocument)
        {
            var userIdClaims = GetUserIdFromClaims();
            var updateResult = await _documentService.UpdateDocument(userIdClaims, updatedDocument);
            if (!updateResult)
                return BadRequest("Failed to update!");


            return NoContent();

        }

        [Authorize(Roles = "User")]
        [HttpDelete("{documentId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {

            var userIdClaims = GetUserIdFromClaims();
            var deletedResult = await _documentService.DeleteDocument(userIdClaims, documentId);
            if (!deletedResult)
                return NotFound("Document not found or already deleted.");

            return NoContent();
        }

    }
}
