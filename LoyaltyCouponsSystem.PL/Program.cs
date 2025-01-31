using IronWord.Models;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Entity.Permission;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using LoyaltyCouponsSystem.DAL.Repo.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static LoyaltyCouponsSystem.PL.Controllers.AdminController;

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

            // Add service to generate QR codes
            builder.Services.AddScoped<IQRCodeGeneratorHelper, QRCodeGeneratorHelper>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect to login page
                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page
            });

            // Add authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Manage Customers", policy => policy.Requirements.Add(new PermissionRequirement("Manage Customers")));
                options.AddPolicy("Manage Users", policy => policy.Requirements.Add(new PermissionRequirement("Manage Users")));
                options.AddPolicy("Exchange Permissions", policy => policy.Requirements.Add(new PermissionRequirement("Exchange Permissions")));
                options.AddPolicy("Generate QR Codes", policy => policy.Requirements.Add(new PermissionRequirement("Generate QR Codes")));
                options.AddPolicy("Scan QR Codes", policy => policy.Requirements.Add(new PermissionRequirement("Scan QR Codes")));
                options.AddPolicy("Deliver From Representative to Customer", policy => policy.Requirements.Add(new PermissionRequirement("Deliver From Representative to Customer")));
                options.AddPolicy("Generate QR Codes", policy => policy.Requirements.Add(new PermissionRequirement("Generate QR Codes")));
            });

            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
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

            // Ensure roles, the initial SuperAdmin user, and permissions exist at startup
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await CreateRolesAndSuperAdminAsync(roleManager, userManager);
                await SeedPermissionsAsync(roleManager, context); // Seed Permissions to Roles
                await AssignPermissionsToUsersWithRoleAsync( roleManager,userManager,context); // Assign Permissions to Roles
            
            }

            app.Run();
        }

        private static async Task CreateRolesAndSuperAdminAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roleNames = { "Admin", "HR", "Representative", "Storekeeper", "Accountant", "SuperAdmin" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

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
                    EmailConfirmed = true,
                    FullName = "Super Admin"
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

        private static async Task SeedPermissionsAsync(
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            var permissions = new List<string>
            {
                "Manage Customers",
                "Manage Users", "Generate QR Codes", "Exchange Permissions","Receive From Customer","Deliver From Representative to Customer","Approve Recieved Coupons"
            };

            foreach (var permissionName in permissions)
            {
                var permission = await context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == permissionName);
                if (permission == null)
                {
                    permission = new Permission { Name = permissionName };
                    context.Permissions.Add(permission);
                    await context.SaveChangesAsync();
                }
            }
        }

      
        private static async Task AssignPermissionsToUsersWithRoleAsync(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context)
        {
            var roleNamesToPermissions = new Dictionary<string, List<string>>
    {
        { "Admin", new List<string> { "Manage Customers",  "Manage Users", "Generate QR Codes", "Exchange Permissions" } },
        { "HR", new List<string> { "Manage Users" } },
        { "Representative", new List<string> { "Manage Customers" } },
        { "Storekeeper", new List<string> { "Exchange Permissions" } },
        { "Accountant", new List<string> { "Manage Users" } },
        { "SuperAdmin", new List<string> { "Manage Customers",  "Manage Users", "Generate QR Codes", "Exchange Permissions", "Scan QR Codes" , "Deliver From Representative to Customer", "Receive From Customer", "Approve Recieved Coupons" } }
    };

            foreach (var roleName in roleNamesToPermissions)
            {
                var role = await roleManager.FindByNameAsync(roleName.Key);
                if (role != null)
                {
                    // Get all users who have the role
                    var usersWithRole = await userManager.GetUsersInRoleAsync(role.Name);

                    foreach (var user in usersWithRole)
                    {
                        foreach (var permissionName in roleName.Value)
                        {
                            var permission = await context.Permissions
                                .FirstOrDefaultAsync(p => p.Name == permissionName);

                            if (permission != null)
                            {
                                var existingUserPermission = await context.UserPermissions
                                    .FirstOrDefaultAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id);

                                if (existingUserPermission == null)
                                {
                                    var userPermission = new UserPermission
                                    {
                                        UserId = user.Id,
                                        PermissionId = permission.Id,
                                    };
                                    context.UserPermissions.Add(userPermission);
                                }
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }


    }
}
