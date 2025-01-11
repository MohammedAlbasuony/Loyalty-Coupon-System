using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Repo.Implementation
{
    public class DistributorRepo : IDistributorRepo
    {
        private readonly ApplicationDbContext _DBcontext;

        public DistributorRepo(ApplicationDbContext context)
        {
            _DBcontext = context;
        }

        // Add method
        public async Task<bool> AddAsync(Distributor distributor)
        {
            try
            {
                await _DBcontext.Distributors.AddAsync(distributor);
                await _DBcontext.SaveChangesAsync();
                return true;
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
                var distributor = await _DBcontext.Distributors.Where(d => d.DistributorID == id).FirstOrDefaultAsync();
                if (distributor != null)
                {
                    _DBcontext.Distributors.Remove(distributor);
                    await _DBcontext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting distributor: {ex.Message}");
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
