using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using LoyaltyCouponsSystem.DAL.Repo.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL
{
    public class Program
    {
        private static void ConfigurePasswordOptions(IdentityOptions options)
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;

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
            builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ITechnicianRepo, TechnicianRepo>();
            builder.Services.AddScoped<ITechnicianService, TechnicianService>();


            //Add service make QRCode to the container
            builder.Services.AddScoped<IQRCodeGeneratorHelper, QRCodeGeneratorHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Ensure roles exist at startup
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await CreateRolesAsync(roleManager);
            }

            app.Run();
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "HR", "Representative", "Storekeeper", "Accountant", "SuperAdmin" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
