using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DocumentController  : Controller
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService) 
        {
            _documentService = documentService;
        }

        [HttpGet("Workspace/{workspaceId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<DocumentDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDocumentsByWorkspaceId(int workspaceId)
        {
           var folders = await _documentService.GetDocumentsByWorkspaceId(workspaceId);
            if(folders == null || !folders.Any())
            {
                return NotFound();
            }
            return Ok(folders);
        }


    }
}
