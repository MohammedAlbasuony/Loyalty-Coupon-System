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
                    ApplicationUser users = new ApplicationUser
                    {
                        FullName = model.Name,
                        Email = model.Email,
                        UserName = model.Email,
                    };

                    var result = await userManager.CreateAsync(users, model.Password);

                    if (result.Succeeded)
                    {
                          
                            var resultrole = await userManager.AddToRoleAsync(users, "REPRESENTATIVE");
                         
                        return RedirectToAction("Login", "Account");
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
