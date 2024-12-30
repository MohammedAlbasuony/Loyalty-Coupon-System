using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // View all users
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users); ;
        }

        // Edit user
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminService.UpdateUserAsync(model);
                if (result) return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        [HttpPost]
        // Delete user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var result = await _adminService.DeleteUserAsync(id);
            if (result)
            {
                return RedirectToAction("ManageUsers");
            }
            else
            {
                return NotFound();  // Return 404 if the user is not found or deletion fails
            }
        }

        
        // Assign Role 
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest("User ID and role name are required.");
            }

            var result = await _adminService.AssignRoleToUserAsync(userId, roleName);
            if (result)
            {
                return Ok("Role assigned successfully.");
            }
            else
            {
                return BadRequest("Failed to assign role.");
            }
        }
        public async Task<IActionResult> AssignRole()
        {
            // Fetch all users from the admin service
            var users = await _adminService.GetAllUsersAsync();

            // Return the view, passing the list of users
            return View(users);
        }
    }
}
