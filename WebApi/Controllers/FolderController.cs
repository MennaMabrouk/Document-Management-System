using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class FolderController : BaseController
    {
        private readonly IFolderService _folderService;
        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("PublicFolders")]
        [ProducesResponseType(200, Type = typeof(ICollection<FolderDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetAllPublicFolders()
        {
            var userIdClaims = GetUserIdFromClaims();
            var publicFolders = await _folderService.GetAllPublicFolders(userIdClaims);
            if (!publicFolders.Any())
                return NoContent();

            return Ok(publicFolders);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{folderId}")]
        [ProducesResponseType(200, Type = typeof(FolderDto))]
        public async Task<IActionResult> GetFolderById(int folderId)
        {
            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var folder = await _folderService.GetFolderById(userIdClaims, folderId, roles);

            return Ok(folder);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("Workspace/{workspaceId}")]
        [ProducesResponseType(200, Type = typeof(ICollection<FolderDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetAllFoldersByWorkspaceId(int workspaceId)
        {

            var userIdClaims = GetUserIdFromClaims();
            var roles = GetRoleFromClaims();
            var Folders = await _folderService.GetAllFoldersByWorkspaceId(userIdClaims, workspaceId, roles);
            if (!Folders.Any())
                return NoContent();


            return Ok(Folders);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> CreateFolder([FromBody] FolderDto createdFolder)
        {

            var userIdClaims = GetUserIdFromClaims();
            var createResult = await _folderService.CreateFolder(userIdClaims, createdFolder);
            if (!createResult)
                return BadRequest("Failed to create the folder.");

            return CreatedAtAction(nameof(GetFolderById), new { folderId = createdFolder.FolderId }, createdFolder);
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> UpdateFolder([FromBody] FolderDto folderDto)
        {
            var userIdClaims = GetUserIdFromClaims();
            var updateResult = await _folderService.UpdateFolder(userIdClaims, folderDto);
            if (!updateResult)
                return BadRequest("Failed to update the folder.");

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{folderId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {

            var userIdClaims = GetUserIdFromClaims();
            var deletedResult = await _folderService.DeleteFolder(userIdClaims, folderId);
            if (!deletedResult)
                return BadRequest("Failed to delete the folder.");

            return NoContent();
        }

        /* [HttpPatch("Restore/Workspace/{workspaceId}")]
         [ProducesResponseType(204)]
         [ProducesResponseType(404)]
         public async Task<IActionResult> RestoreAllSoftDeletedFoldersByWorkspaceId(int workspaceId)
         {
             var result =await _folderService.RestoreAllSoftDeletedFoldersByWorkspaceId(workspaceId);
             if (result) 
                 {
                     return NoContent();
                 }

             return NotFound("No deleted folders found in the workspace.");
         }

         [HttpPatch("Restore/{folderId}")]
         [ProducesResponseType(204)]
         [ProducesResponseType(404)]
         public async Task<IActionResult> RestoreSoftDeletedFolderById(int folderId)
         {
             var result = await _folderService.RestoreSoftDeletedFolderById(folderId);
             if (result)
             {
                 return NoContent();
             }

             return NotFound("Folder not found or not deleted");
         }*/
    }
}
