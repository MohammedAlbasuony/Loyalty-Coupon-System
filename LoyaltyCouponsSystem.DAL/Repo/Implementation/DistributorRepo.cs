using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace LoyaltyCouponsSystem.DAL.Repo.Implementation
{
    public class DistributorRepo : IDistributorRepo
    {
        private readonly ApplicationDbContext _DBcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DistributorRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _DBcontext = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsUniqueCodeAsync(string code)
        {
            return !await _DBcontext.Distributors.AnyAsync(d => d.Code == code);
        }

        // Check if PhoneNumber1 is unique
        public async Task<bool> IsUniquePhoneNumberAsync(int phoneNumber)
        {
            return !await _DBcontext.Distributors.AnyAsync(d => d.PhoneNumber1 == phoneNumber);
        }
        public async Task<List<int>> GetValidCustomerIdsAsync(List<string> customerCodes)
        {
            return await _DBcontext.Customers
             .Where(c => customerCodes.Contains(c.Code)) // Match codes to IDs
             .Select(c => c.CustomerID)
             .ToListAsync();
        }

        // Add method
        public async Task<bool> AddAsync(Distributor distributor)
        {
            try
            {
                // Ensure the Code and Phone Number are unique
                if (!await IsUniqueCodeAsync(distributor.Code))
                    throw new Exception("Code already exists.");
                if (!await IsUniquePhoneNumberAsync(distributor.PhoneNumber1))
                    throw new Exception("Phone number already exists.");

                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
                distributor.CreatedBy = currentUser?.UserName;
                distributor.CreatedAt = DateTime.UtcNow;
                _DBcontext.Distributors.Add(distributor);
                return await _DBcontext.SaveChangesAsync() > 0; // Returns true only if changes were saved
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding distributor: {ex.Message}");
                return false;
            }
        }

        // Delete method
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {

                var user = await _DBcontext.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsDeleted = true;
                    await _DBcontext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        // Get all method
        public async Task<List<Distributor>> GetAllAsync()
        {
            try
            {
                return await _DBcontext.Distributors
                    .Include(d => d.DistributorCustomers).ThenInclude(x => x.Customer) // if related data is required
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching distributors: {ex.Message}");
                return new List<Distributor>();
            }
        }

        // Get by ID method
        public async Task<Distributor> GetByIdAsync(int id)
        {
            try
            {
                return await _DBcontext.Distributors.Where(d => d.DistributorID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching distributor by ID: {ex.Message}");
                return null;
            }
        }

        // Update method
        public async Task<bool> UpdateAsync(Distributor distributor)
        {
            try
            {
                var existingDistributor = await _DBcontext.Distributors
                .Where(d => d.DistributorID == distributor.DistributorID && d.IsDeleted == false)
                .FirstOrDefaultAsync();


                if (existingDistributor == null)
                {
                    return false;
                }

                // Update properties
                existingDistributor.Name = distributor.Name;
                existingDistributor.PhoneNumber1 = distributor.PhoneNumber1;
                existingDistributor.Governate = distributor.Governate;
                existingDistributor.City = distributor.City;

                _DBcontext.Update(existingDistributor);
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating distributor: {ex.Message}");
                return false;
            }
        }

        // Get customers for dropdown (names and codes)
        public async Task<List<Customer>> GetCustomersForDropdownAsync()
        {
            try
            {
                return await _DBcontext.Customers
                    .Select(c => new Customer { Name = c.Name, Code = c.Code })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers for dropdown: {ex.Message}");
                return new List<Customer>();
            }
        }


    }
}
