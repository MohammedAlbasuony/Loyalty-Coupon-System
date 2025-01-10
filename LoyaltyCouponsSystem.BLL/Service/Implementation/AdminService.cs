using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Admin;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
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

    }
}
