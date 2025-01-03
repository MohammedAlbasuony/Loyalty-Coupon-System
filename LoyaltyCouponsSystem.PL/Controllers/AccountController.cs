namespace LoyaltyCouponsSystem.PL.Controllers
{
    using Loyality_Copoun_System.Models;
    using LoyaltyCouponsSystem.BLL.ViewModel.Account;
    using LoyaltyCouponsSystem.DAL.DB;
    using LoyaltyCouponsSystem.DAL.Migrations;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

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
                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email or password is incorrect.");
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
                    ApplicationUser user = new ApplicationUser
                    {
                        FullName = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.PhoneNumber, // Using PhoneNumber as UserName
                        Governorate = model.Governorate,
                        City = model.City,
                        NationalID = model.NationalID
                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var roleResult = await userManager.AddToRoleAsync(user, "REPRESENTATIVE");

                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }

                return View(model);
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
                    var user = await userManager.FindByNameAsync(model.Email);

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
                return View(new ChangePasswordViewModel { Email = username });
            }

            [HttpPost]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Email);
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
