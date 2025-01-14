using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
                distributor.CreatedBy = currentUser?.UserName;
                distributor.CreatedAt = DateTime.UtcNow;
                _DBcontext.Distributors.Add(distributor);
                return await _DBcontext.SaveChangesAsync() > 0; // Returns true only if changes were saved
            }
            catch (Exception ex)
            {
                // Log exception here
                Console.WriteLine(ex.Message);
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
                return await _DBcontext.Distributors.ToListAsync();
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
                    .Where(d => d.DistributorID == distributor.DistributorID)
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
