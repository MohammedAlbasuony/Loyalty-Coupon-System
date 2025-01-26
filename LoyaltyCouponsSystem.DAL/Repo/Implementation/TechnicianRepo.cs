using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace LoyaltyCouponsSystem.DAL.Repo.Implementation
{
    public class TechnicianRepo : ITechnicianRepo
    {
        private readonly ApplicationDbContext _DBcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TechnicianRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _DBcontext = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsUniqueCodeAsync(string code)
        {
            return !await _DBcontext.Technicians.AnyAsync(t => t.Code == code);
        }

        // Check if PhoneNumber1 is unique
        public async Task<bool> IsUniquePhoneNumberAsync(int phoneNumber)
        {
            return !await _DBcontext.Technicians.AnyAsync(t => t.PhoneNumber1 == phoneNumber);
        }
        public async Task<List<int>> GetValidCustomerIdsAsync(List<string> customerCodes)
        {
            return await _DBcontext.Customers
             .Where(c => customerCodes.Contains(c.Code)) // Match codes to IDs
             .Select(c => c.CustomerID)
             .ToListAsync();
        }
        public async Task<List<string>> GetValidUserIdsAsync(List<string> userNamesOrEmails)
        {
            var users = await _userManager.Users
                .Where(u => userNamesOrEmails.Contains(u.UserName) || userNamesOrEmails.Contains(u.Email))
                .Select(u => u.Id)
                .ToListAsync();

            return users;
        }
        // Add method
        public async Task<bool> AddAsync(Technician technician)
        {
            try
            {
                // Ensure the Code and Phone Number are unique
                if (!await IsUniqueCodeAsync(technician.Code))
                    throw new Exception("Code already exists.");
                if (!await IsUniquePhoneNumberAsync(technician.PhoneNumber1))
                    throw new Exception("Phone number already exists.");

                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
                technician.CreatedBy = currentUser?.UserName;
                technician.CreatedAt = DateTime.Now;
                await _DBcontext.Technicians.AddAsync(technician);
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding technician: {ex.Message}");
                return false;
            }
        }

        // Delete method
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var technician = await _DBcontext.Technicians.Where(t => t.TechnicianID == id).FirstOrDefaultAsync();
                if (technician != null)
                {
                    _DBcontext.Technicians.Remove(technician);
                    await _DBcontext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting technician: {ex.Message}");
                return false;
            }
        }

        // Get all method
        public async Task<List<Technician>> GetAllAsync()
        {
            try
            {
                // Fetch all Technicians
                var technicians = await _DBcontext.Technicians.ToListAsync();

                // Fetch all Users (using UserManager)
                var users = await _userManager.Users.ToListAsync();

                // Fetch all Customers
                var customers = await _DBcontext.Customers.ToListAsync();

                // Iterate through each Technician
                foreach (var technician in technicians)
                {
                    foreach (var user in technician.Users)
                    {
                        // Find the user with the same ID in the Users list
                        var relatedUser = users.FirstOrDefault(u => u.Id == user.Id);

                        // If user exists, assign its UserName or else set "No User"
                        user.UserName = relatedUser?.UserName ?? "No User";
                    }
                }

                return technicians;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching technicians: {ex.Message}");
                return new List<Technician>();
            }
        }

        // Get by ID method
        public async Task<Technician> GetByIdAsync(int id)
        {
            try
            {
                return await _DBcontext.Technicians.Where(t => t.TechnicianID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching technician by ID: {ex.Message}");
                return null;
            }
        }

        // Update method
        public async Task<bool> UpdateAsync(Technician technician)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
                technician.UpdatedBy = currentUser?.UserName;
                technician.UpdatedAt = DateTime.Now;
                var existingTechnician = await _DBcontext.Technicians
                    .Where(t => t.TechnicianID == technician.TechnicianID)
                    .FirstOrDefaultAsync();

                if (existingTechnician == null)
                {
                    return false;
                }

                // Update properties
                existingTechnician.TechnicianID = technician.TechnicianID;
                existingTechnician.Name = technician.Name;
                existingTechnician.NickName = technician.NickName;
                existingTechnician.NationalID = technician.NationalID;
                existingTechnician.PhoneNumber1 = technician.PhoneNumber1;
                existingTechnician.PhoneNumber2 = technician.PhoneNumber2;
                existingTechnician.PhoneNumber3 = technician.PhoneNumber3;
                existingTechnician.Governate = technician.Governate;
                existingTechnician.City = technician.City;
                existingTechnician.Code = technician.Code;
                existingTechnician.Users = technician.Users;
                existingTechnician.Customers = technician.Customers;
                existingTechnician.UpdatedAt = DateTime.Now;
                existingTechnician.UpdatedBy = technician.UpdatedBy;
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating technician: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Customer>> GetCustomersForDropdownAsync()
        {
            try
            {
                return await _DBcontext.Customers
                    .Where(c => c.IsActive) // Filter only active customers
                    .Select(c => new Customer
                    {
                        Name = c.Name,
                        Code = c.Code
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers for dropdown: {ex.Message}");
                return new List<Customer>();
            }
        }

        public async Task<List<ApplicationUser>> GetUsersForDropdownAsync()
        {
            try
            {
                return await _userManager.Users
                    .Select(u => new ApplicationUser { UserName = u.UserName, Id = u.Id }) // Adjust properties as needed
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users for dropdown: {ex.Message}");
                return new List<ApplicationUser>();
            }
        }

        public async Task AssignCustomerAsync(int technicianId, int customerId)
        {
            var technician = await _DBcontext.Technicians
                .Include(t => t.Customers)
                .FirstOrDefaultAsync(t => t.TechnicianID == technicianId);

            if (technician != null && !technician.Customers.Any(c => c.CustomerID == customerId))
            {
                var customer = await _DBcontext.Customers.FindAsync(customerId);
                if (customer != null)
                {
                    technician.Customers.Add(customer);
                    await _DBcontext.SaveChangesAsync();
                }
            }
        }

        // Remove Customer
        public async Task RemoveCustomerByNameAsync(int technicianId, string customerName)
        {
            var technician = await _DBcontext.Technicians
                .Include(t => t.Customers)
                .FirstOrDefaultAsync(t => t.TechnicianID == technicianId);

            if (technician != null)
            {
                // Find the customer by name
                var customer = technician.Customers.FirstOrDefault(c => c.Name == customerName);

                if (customer != null)
                {
                    // Remove the customer by its ID
                    technician.Customers.Remove(customer);
                    await _DBcontext.SaveChangesAsync();
                }
            }
        }


        // Get All Active Customers
        public async Task<List<Customer>> GetActiveUnassignedCustomersAsync()
        {
            return await _DBcontext.Customers
                .Where(c => c.IsActive && !c.TechnicianId.HasValue) // Only active customers with no assigned technician
                .ToListAsync();
        }




        // Assign User with Role "Representative"
        public async Task AssignUserAsync(int technicianId, string userId)
        {
            var technician = await _DBcontext.Technicians
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.TechnicianID == technicianId);

            if (technician != null && !technician.Users.Any(u => u.Id == userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    technician.Users.Add(user);
                    await _DBcontext.SaveChangesAsync();
                }
            }
        }

        // Remove User
        public async Task RemoveUserAsync(int technicianId, string userId)
        {
            var technician = await _DBcontext.Technicians
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.TechnicianID == technicianId);

            if (technician != null)
            {
                var user = technician.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    technician.Users.Remove(user);
                    await _DBcontext.SaveChangesAsync();
                }
            }
        }

        // Get All Users with Role "Representative"
        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return users.ToList();
        }

    }
}
