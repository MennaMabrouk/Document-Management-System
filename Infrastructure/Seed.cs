using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public class Seed
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public Seed(DataContext dataContext, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            // Ensure roles exist
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" });
                await _roleManager.CreateAsync(new IdentityRole<int> { Name = "User", NormalizedName = "USER" });
            }

            var users = _userManager.Users.ToList();  // Pull all users into memory
            if (!users.Any(u => _userManager.IsInRoleAsync(u, "Admin").Result))
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Nid = "12345678901234",
                    Gender = "Male",
                    YearOfBirth = 1980
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!users.Any(u => _userManager.IsInRoleAsync(u, "User").Result))
            {
                var regularUser = new User
                {
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Nid = "98765432109876",
                    Gender = "Female",
                    YearOfBirth = 1995
                };

                var result = await _userManager.CreateAsync(regularUser, "User123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(regularUser, "User");

                    // Create a workspace for the regular user
                    var workspace = new Workspace
                    {
                        Name = "UserWorkspace",
                        CreationDate = DateTime.UtcNow,
                        UserId = regularUser.Id
                    };
                    _dataContext.Workspaces.Add(workspace);
                    await _dataContext.SaveChangesAsync();

                    // Optionally create a folder and document if needed
                    var folder = new Folder
                    {
                        Name = "UserFolder",
                        CreationDate = DateTime.UtcNow,
                        IsPublic = true,
                        WorkspaceId = workspace.WorkspaceId
                    };
                    _dataContext.Folders.Add(folder);
                    await _dataContext.SaveChangesAsync();

                    var document = new Document
                    {
                        Name = "UserDocument",
                        CreationDate = DateTime.UtcNow,
                        IsDeleted = false,
                        Type = "txt",
                        Version = "1.0",
                        Tag = "General",
                        AccessControl = "User",
                        FolderId = folder.FolderId
                    };
                    _dataContext.Documents.Add(document);
                    await _dataContext.SaveChangesAsync();
                }
            }
        }
    }
}
