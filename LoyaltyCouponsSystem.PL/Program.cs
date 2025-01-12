using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using LoyaltyCouponsSystem.DAL.Repo.Implementation;
using LoyaltyCouponsSystem.DAL.Repo.Implementation.LoyaltyCouponsSystem.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL
{
    public class Program
    {
        private static void ConfigurePasswordOptions(IdentityOptions options)
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Allow spaces in UserName
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register the DbContext service
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                ConfigurePasswordOptions(options);
                options.SignIn.RequireConfirmedAccount = true; // Email confirmation
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Register repositories and services
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ITechnicianRepo, TechnicianRepo>();
            builder.Services.AddScoped<ITechnicianService, TechnicianService>();
            builder.Services.AddScoped<IExchangeOrderRepo, ExchangeOrderRepo>();
            builder.Services.AddScoped<IExchangeOrderService, ExchangeOrderService>();
            builder.Services.AddScoped<IDistributorService, DistributorService>();
            builder.Services.AddScoped<IDistributorRepo, DistributorRepo>();

            // Add service make QRCode to the container
            builder.Services.AddScoped<IQRCodeGeneratorHelper, QRCodeGeneratorHelper>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect to login page
                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Ensure roles and the initial SuperAdmin user exist at startup
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await CreateRolesAndSuperAdminAsync(roleManager, userManager);
            }

            app.Run();
        }

        private static async Task CreateRolesAndSuperAdminAsync(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager)
        {
            // Roles to ensure exist
            string[] roleNames = { "Admin", "HR", "Representative", "Storekeeper", "Accountant", "SuperAdmin" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create the initial SuperAdmin user if not already present
            var superAdminEmail = "superadmin@example.com";
            var superAdminUserName = "superadmin";
            var superAdminPassword = "SuperAdmin@Password_2025!";

            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdmin == null)
            {
                var newSuperAdmin = new ApplicationUser
                {
                    UserName = superAdminUserName,
                    Email = superAdminEmail,
                    EmailConfirmed = true, // Skip email confirmation for the initial user
                    FullName = "Super Admin" // Provide a default FullName value
                };

                var createResult = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                    Console.WriteLine("SuperAdmin user created successfully.");
                }
                else
                {
                    Console.WriteLine("Error creating SuperAdmin user:");
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("SuperAdmin user already exists.");
            }
        }

    }
}
