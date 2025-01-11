namespace LoyaltyCouponsSystem.PL.Controllers
{
    using LoyaltyCouponsSystem.BLL.Service.Abstraction;
    using LoyaltyCouponsSystem.BLL.ViewModel.Account;
    using LoyaltyCouponsSystem.DAL.DB;
    
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    namespace UsersApp.Controllers
    {
        public class AccountController : Controller
        {
            private readonly SignInManager<ApplicationUser> signInManager;
            private readonly UserManager<ApplicationUser> userManager;
            private readonly ApplicationDbContext _context;
            private readonly IAccountService _accountService;

            public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IAccountService accountService)
            {
                this.signInManager = signInManager;
                this.userManager = userManager;
                _context = context;
                _accountService = accountService;
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
                        var userDetails = _accountService.GetUserByUsernameAsync(model.Name); 
                        TempData["UserDetails"] = JsonConvert.SerializeObject(userDetails);
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
                // Set the Role to "Representative" by default
                var model = new RegisterViewModel
                {
                    Role = "Representative" // Default value
                };
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    // Check if NationalID already exists
                    var existingUserByNationalID = await userManager.Users.FirstOrDefaultAsync(u => u.NationalID == model.NationalID);
                    if (existingUserByNationalID != null)
                    {
                        ModelState.AddModelError("NationalID", "The National ID is already in use.");
                        return View(model);
                    }

                    // Check if PhoneNumber already exists
                    var existingUserByPhoneNumber = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                    if (existingUserByPhoneNumber != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "The phone number is already in use.");
                        return View(model);
                    }

                    // Check if OptionalPhoneNumber is provided and already exists
                    if (!string.IsNullOrEmpty(model.OptionalPhoneNumber))
                    {
                        var existingUserByOptionalPhone = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.OptionalPhoneNumber);
                        if (existingUserByOptionalPhone != null)
                        {
                            ModelState.AddModelError("OptionalPhoneNumber", "The optional phone number is already in use.");
                            return View(model);
                        }
                    }

                    var user = new ApplicationUser
                    {
                        FullName = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Governorate = model.Governorate,
                        City = model.City,
                        NationalID = model.NationalID,
                        Role = "Representative",
                        UserName = model.Name,
                        OptionalPhoneNumber = model.OptionalPhoneNumber


                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Assign the "Representative" role
                        await userManager.AddToRoleAsync(user, "Representative");
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
            [Route("api/get-cities")]
            public IActionResult GetCities(string governorate)
            {
                var cities = new Dictionary<string, List<string>>
                {
                    { "Cairo", new List<string> { "Nasr City", "Heliopolis", "Maadi", "New Cairo" } },
                    { "Alexandria", new List<string> { "Montaza", "Smouha", "Stanley" } },
                    { "Giza", new List<string> { "Dokki", "Haram", "6th of October" } },
                    { "Dakahlia", new List<string> { "Mansoura", "Mit Ghamr", "Talkha" } },
                    { "Red Sea", new List<string> { "Hurghada", "Safaga", "El Qoseir" } },
                    { "Beheira", new List<string> { "Damanhour", "Rashid", "Kafr El-Dawwar" } },
                    { "Fayoum", new List<string> { "Fayoum City", "Tamiya", "Ibsheway" } },
                    { "Sharqia", new List<string> { "Zagazig", "Belbeis", "10th of Ramadan City" } },
                    { "Aswan", new List<string> { "Aswan City", "Kom Ombo", "Edfu" } },
                    { "Assiut", new List<string> { "Assiut City", "Manfalut", "Dayrout" } },
                    { "Beni Suef", new List<string> { "Beni Suef City", "Nasser", "Ihnasya" } },
                    { "Port Said", new List<string> { "Port Said City" } },
                    { "Suez", new List<string> { "Suez City" } },
                    { "Matruh", new List<string> { "Marsa Matruh", "Siwa", "Alamein" } },
                    { "Qalyubia", new List<string> { "Banha", "Shubra El-Kheima", "Qalyub" } },
                    { "Gharbia", new List<string> { "Tanta", "El-Mahalla", "Kafr El-Zayat" } },
                    { "Monufia", new List<string> { "Shebin El-Kom", "Sadat City", "Menouf" } },
                    { "Qena", new List<string> { "Qena City", "Nag Hammadi", "Qus" } },
                    { "North Sinai", new List<string> { "Arish", "Sheikh Zuweid", "Rafah" } },
                    { "Sohag", new List<string> { "Sohag City", "Tahta", "Akhmim" } },
                    { "South Sinai", new List<string> { "Sharm El-Sheikh", "Dahab", "Taba" } },
                    { "Kafr El Sheikh", new List<string> { "Kafr El Sheikh City", "Desouk", "Balteem" } },
                    { "Damietta", new List<string> { "Damietta City", "Ras El Bar", "New Damietta" } },
                    { "Ismailia", new List<string> { "Ismailia City", "Fayed", "Qantara" } },
                    { "Luxor", new List<string> { "Luxor City", "Armant", "Esna" } },
                    { "New Valley", new List<string> { "Kharga", "Dakhla", "Farafra" } }
                };

                if (cities.ContainsKey(governorate))
                {
                    return Json(cities[governorate]);
                }

                return Json(new List<string>()); // Return an empty list if no cities found
            }




            [HttpGet]
            [Route("api/check-phone-number")]
            public IActionResult CheckPhoneNumber(string phoneNumber)
            {
                // Check if the phone number already exists in the database
                var isUnique = !_context.Users.Any(u => u.PhoneNumber == phoneNumber);
                return Json(new { isUnique });
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
