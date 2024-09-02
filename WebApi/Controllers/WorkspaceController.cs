using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
  
    public class WorkspaceController : BaseController
    {
        private readonly IWorkspaceService _workspaceService;


        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;

        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(WorkspaceDto))]

        public async Task<IActionResult> GetWorkspaceWithFoldersForUser(int userId)
        {
            var userIdClaims = GetUserIdFromClaims();
            var role = GetRoleFromClaims();

            var workspace = await _workspaceService.GetWorkspaceByUserId(userIdClaims, userId, role);

            return Ok(workspace);

        }

        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> UpdateWorkspace([FromBody] WorkspaceDto updatedWorkspace)
        {
            var userIdClaims = GetUserIdFromClaims();
            var updateResult = await _workspaceService.UpdateWorkspace(userIdClaims, updatedWorkspace);
            if (!updateResult)
                return BadRequest("Failed to update!");


            return NoContent();

        }



    }
}
