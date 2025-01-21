using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Admin;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Entity.Permission;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _DBcontext;

        public AdminService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _DBcontext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<Representative>> GetAllRepresentativesAsync()
        {
            return await dbContext.Representatives
                .Include(r => r.ApplicationUser) // Include related ApplicationUser data
                .Include(r => r.Coupons) // Include related Coupons if necessary
                .ToListAsync();
        }


        public async Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync()
        {
            var users = await dbContext.Users
        .Select(user => new AdminUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Role = user.Role,
            IsDeleted = user.IsDeleted
        }).ToListAsync();

            return users;
        }

        public async Task<AdminUserViewModel> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
            };
        }
        public async Task<bool> UpdateUserAsync(AdminUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.Email = model.Email;
            user.UserName = model.UserName;
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);

            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(e => e.Description);

            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return false;

                // Set IsDeleted to true in ApplicationUser
                user.IsDeleted = true;

                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return true;
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
            //if result succed true 
            // create mmethod in user repo get two parameter
            //first one user id second role namme 
        }


        public async Task<bool> UpdateUserRoleName(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Role = roleName;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();

            return true;
        }



        // Permission

        // Get all permissions for a role
        public async Task<List<string>> GetPermissionsForRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new List<string>();

            var rolePermissions = await _DBcontext.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Include(rp => rp.Permission)
                .ToListAsync();

            return rolePermissions.Select(rp => rp.Permission.Name).ToList();
        }

        // Assign permission to a role
        public async Task<bool> AssignPermissionToRoleAsync(string roleName, string permissionName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return false;

            var permission = await _DBcontext.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                return false;

            var rolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };

            await _DBcontext.RolePermissions.AddAsync(rolePermission);
            await _DBcontext.SaveChangesAsync();

            return true;
        }

        // Remove permission from a role
        public async Task<bool> RemovePermissionFromRoleAsync(string roleName, string permissionName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return false;

            var permission = await _DBcontext.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);
            if (permission == null)
                return false;

            var rolePermission = await _DBcontext.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);

            if (rolePermission != null)
            {
                _DBcontext.RolePermissions.Remove(rolePermission);
                await _DBcontext.SaveChangesAsync();
            }

            return true;
        }

    }
}
