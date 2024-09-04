using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int GetUserIdFromClaims()
        {
            string userToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userToken, out int userIdInt))
            {
                return userIdInt;
            }

            throw new InvalidOperationException("Invalid user ID in token claims.");
        }

        protected string GetRoleFromClaims()
        {
            string role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(role))
            {
                throw new InvalidOperationException("Role is not found in token claims.");
            }
            return role;
        }
    }
}
