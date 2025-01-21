using LoyaltyCouponsSystem.BLL.ViewModel.Admin;
using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync();
        Task<AdminUserViewModel> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserAsync(AdminUserViewModel model);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);
        Task<bool> UpdateUserRoleName(string userId, string roleName);
        Task<List<string>> GetPermissionsForRoleAsync(string roleName);
        Task<bool> RemovePermissionFromRoleAsync(string roleName, string permissionName);
        Task<bool> AssignPermissionToRoleAsync(string roleName, string permissionName);
    }
}
