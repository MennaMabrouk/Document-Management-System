using Application.Dto;
using Application.Dto.User;
using Application.Interfaces;
using Azure.Core;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace WebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserManager<User> _usermanager;
        private readonly IWorkspaceService _workspaceService;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        public UserController(UserManager<User> usermanager, IWorkspaceService workspaceService,
                               IUserService userService, IConfiguration config)
        {
            _usermanager = usermanager;
            _workspaceService = workspaceService;
            _config = config;
            _userService = userService;

        }


        [HttpPost("register")]

        public async Task<IActionResult> Registeration(RegisterUserDto registerdto)
        {
            if (ModelState.IsValid)
            {
                var workspaceNameExists = await _workspaceService.WorkspaceExists(registerdto.WorkspaceName);
                if (workspaceNameExists)
                {
                    return BadRequest("Workspace name already exists! Please choose different name");
                }

                User user = new User
                {
                    UserName = registerdto.Username,
                    Email = registerdto.Email,
                    Nid = registerdto.Nid,
                    Gender = registerdto.Gender,
                    PhoneNumber = registerdto.PhoneNumber,
                    /*  YearOfBirth = registerdto.YearOfBirth,*/

                    Workspace = new Workspace
                    {
                        Name = registerdto.WorkspaceName,
                        CreationDate = DateTime.UtcNow
                    }

                };



                IdentityResult result = await _usermanager.CreateAsync(user, registerdto.Password);

                if (result.Succeeded)
                {
                    var roleResult = await _usermanager.AddToRoleAsync(user, "User");

                    bool folderCreated = await _userService.CreateWorkspacePathForUser(registerdto.WorkspaceName);

                    if (folderCreated)
                    {
                        return Ok(new { message = "Account added successfully" });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Account created, but failed to create workspace directory." });
                    }
                }

                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Errors = errors });

                }

            }

            /*  return BadRequest(ModelState);*/
            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto logindto)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(logindto.Email);

                if (user != null)
                {

                    if (await _usermanager.IsLockedOutAsync(user))
                    {
                        return StatusCode(403, new { message = $"This account is locked until {user.LockoutEnd.Value.ToLocalTime():MM/dd/yyyy hh:mm tt}" });
                    }


                    bool found = await _usermanager.CheckPasswordAsync(user, logindto.Password);

                    if (found)
                    {

                        //Claims
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Email, user.Email));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        //Get Roles to add in claims
                        var roles = await _usermanager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        //Sign in credentials
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                        string algorithm = _config["JWT:Algorithm"] switch
                        {
                            "HmacSha256" => SecurityAlgorithms.HmacSha256,
                            "HmacSha384" => SecurityAlgorithms.HmacSha384,
                            "HmacSha512" => SecurityAlgorithms.HmacSha512,
                            _ => SecurityAlgorithms.HmacSha256 // Default fallback
                        };

                        var signInCredentials = new SigningCredentials(securityKey, algorithm);

                        //Generate Token
                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            issuer: _config["JWT:ValidIssuer"],
                            audience: _config["JWT:ValidAudience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(2),
                            signingCredentials: signInCredentials
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration = mytoken.ValidTo,
                            role = roles.FirstOrDefault(),
                        });

                    }

                    return Unauthorized();

                }

                return Unauthorized();

            }

            return BadRequest(ModelState);

        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("details")]
        public async Task<IActionResult> GetUserId()
        {
            var userId = GetUserIdFromClaims();
            var user = await _usermanager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }


            var roles = GetRoleFromClaims();

            return Ok(new
            {
                userId = user.Id,

            });
        }




        [Authorize(Roles = "Admin")]
        [HttpPost("lock/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LockUser(int userId, [FromBody] LockUserDto lockRequest)
        {
            var user = await _usermanager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            DateTimeOffset lockoutEnd;

            switch (lockRequest.TimeUnit.ToLower())
            {
                case "hours":
                    lockoutEnd = DateTimeOffset.UtcNow.AddHours(lockRequest.LockTime);
                    break;
                case "minutes":
                    lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockRequest.LockTime);
                    break;
                case "days":
                    lockoutEnd = DateTimeOffset.UtcNow.AddDays(lockRequest.LockTime);
                    break;
                default:
                    lockoutEnd = DateTimeOffset.UtcNow.AddHours(1);
                    break;
            }

            user.LockoutEnd = lockoutEnd;
            user.LockoutEnabled = true;

            var result = await _usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User locked successfully." });
            }

            return BadRequest(new { message = "Failed to lock the user." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("unlock/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]  // 409 Conflict when the user is not locked
        public async Task<IActionResult> UnlockUser(int userId)
        {
            var user = await _usermanager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check if the user is locked
            if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
            {
                return Conflict(new { message = "User is not currently locked." });
            }

            // Remove the lockout
            user.LockoutEnd = null;
            user.LockoutEnabled = false;

            var result = await _usermanager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User unlocked successfully." });
            }

            return BadRequest(new { message = "Failed to unlock the user." });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PaginatedResult<UserDto>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = (int)PaginationEnum.PageNumber, 
                                                    [FromQuery] int pageSize = (int)PaginationEnum.PageSize)
        {
            var paginatedUsers = await _userService.GetPaginatedUsers(pageNumber, pageSize);
            if (!paginatedUsers.Items.Any())
                return NoContent();

            return Ok(paginatedUsers);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var userIdClaims = GetUserIdFromClaims();
            var roleClaims = GetRoleFromClaims();
            var user = await _userService.GetUserById(userIdClaims, userId, roleClaims);

            return Ok(user);

        }

/*
        [Authorize(Roles = "User")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> UpdateUser([FromBody] UserDto updatedUser)
        {

            var userIdClaims = GetUserIdFromClaims();
            var updateResult = await _userService.UpdateUser(userIdClaims, updatedUser);
            if (!updateResult)
                return BadRequest(new { message = "Failed to update!" });


            return NoContent();
        }

*/




        /*        [Authorize(Roles = "User,Admin")]
                [HttpGet("get-user-info")]
                [ProducesResponseType(204)]
                [ProducesResponseType(400)]
                [ProducesResponseType(404)]
                public async Task<IActionResult> GetLoggedUserInfo()
                {
                    var userIdClaims = GetUserIdFromClaims();
                    var roleClaims = GetRoleFromClaims();
                    var userDetails = await _userService.GetUserById(userIdClaims, userIdClaims, roleClaims);

                    if (userDetails == null)
                    {
                        return NotFound("User not found.");
                    }

                    return Ok(new
                    {
                        User = userDetails,
                        Role = roleClaims
                    });

                }*/



        /* [Authorize(Roles = "User")]
         [HttpGet("Workspace/{workspaceId}")]
         [ProducesResponseType(200, Type = typeof(UserDto))]
         [ProducesResponseType(403)]
         [ProducesResponseType(404)]
         public async Task<IActionResult> GetUserByWorkspaceId(int workspaceId)
         {

             var user = await _userService.GetUserByWorkspaceId(workspaceId);
             var userIdClaims = GetUserIdFromClaims();

             if (userIdClaims != user.Id)
                 return Forbid("You are not authorized to access this user info.");

             if (!ModelState.IsValid)
                 return BadRequest(ModelState);

             if (user == null)
                 return NotFound();

             return Ok(user);
         }
 */

    }

}
