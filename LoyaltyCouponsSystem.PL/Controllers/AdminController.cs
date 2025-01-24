using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Admin;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IAdminService adminService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminService = adminService;
        }



        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var confirmedUsers = new List<AdminUserViewModel>();
            var unconfirmedUsers = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userModel = new AdminUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    NationalID = user.NationalID,
                    PhoneNumber = user.PhoneNumber,
                    OptionalPhoneNumber = user.OptionalPhoneNumber,
                    Governorate = user.Governorate,
                    City = user.City,
                    Role = roles.FirstOrDefault(),
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive // Assuming IsActive is part of the ApplicationUser class
                };

                if (user.EmailConfirmed == true)
                    confirmedUsers.Add(userModel);
                else
                    unconfirmedUsers.Add(userModel);
            }

            var model = new ManageUsersViewModel
            {
                ConfirmedUsers = confirmedUsers,
                UnconfirmedUsers = unconfirmedUsers,
                AllRoles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActivation(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive; // Toggle the IsActive property
                await _userManager.UpdateAsync(user);

                // Optionally, add logging or other actions here
            }

            return RedirectToAction("ManageUsers");
        }


        // Edit user
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            user.EmailConfirmed = true;
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

        [HttpGet]
        public async Task<IActionResult> AssignRoleForm()
        {
            var users = await _adminService.GetAllUsersAsync();
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            foreach (var user in users)
            {
                user.Roles = roles;
                user.SelectedRole = user.Role;
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!resultRemove.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to remove existing roles.";
                return RedirectToAction("ManageUsers");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
                TempData["SuccessMessage"] = $"Role '{roleName}' has been assigned to user.";
            else
                TempData["ErrorMessage"] = "Failed to assign the role.";

            var userRoleName = await _adminService.UpdateUserRoleName(userId, roleName);

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User's account has been confirmed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to confirm the account.";
            }

            return RedirectToAction("ManageUsers");
        }
    }
}