namespace LoyaltyCouponsSystem.PL.Controllers
{
    using LoyaltyCouponsSystem.BLL.ViewModel.Account;
    using LoyaltyCouponsSystem.DAL.DB;
    using LoyaltyCouponsSystem.DAL.Migrations;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace UsersApp.Controllers
    {
        public class AccountController : Controller
        {
            private readonly SignInManager<ApplicationUser> signInManager;
            private readonly UserManager<ApplicationUser> userManager;

            public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
            {
                this.signInManager = signInManager;
                this.userManager = userManager;
            }

            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Login(LoginViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var result = await signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login credentials. Please try again!");
                        return View(model);
                    }
                }
                return View(model);
            }

            public IActionResult Register()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    // Check if NationalID already exists
                    var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("NationalID", "The National ID is already in use.");
                        return View(model);
                    }

                    var user = new ApplicationUser
                    {
                        FullName = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Governorate = model.Governorate,
                        City = model.City,
                        NationalID = model.NationalID,
                        Role = model.Role,
                        UserName = model.Name // or any desired logic
                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, model.Role);

                        ViewBag.RegistrationSuccess = true;
                        return View("Register");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                ViewBag.RegistrationSuccess = false;
                return View(model);
            }

            [HttpGet]
            public async Task<IActionResult> CheckNationalID(string nationalID)
            {
                var exists = await userManager.Users.AnyAsync(u => u.NationalID == nationalID);
                return Json(new { isUnique = !exists });
            }

            public IActionResult VerifyEmail()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Name);

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email is not correct!");
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                    }
                }
                return View(model);
            }

            public IActionResult ChangePassword(string username)
            {
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("VerifyEmail", "Account");
                }
                return View(new ChangePasswordViewModel { Name = username });
            }

            [HttpPost]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Name);
                    if (user != null)
                    {
                        var result = await userManager.RemovePasswordAsync(user);
                        if (result.Succeeded)
                        {
                            result = await userManager.AddPasswordAsync(user, model.NewPassword);
                            return RedirectToAction("Log    in", "Account");
                        }
                        else
                        {

                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }

                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email not found!");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong. try again.");
                    return View(model);
                }
            }

            public async Task<IActionResult> Logout()
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
