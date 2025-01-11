using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.DAL.Repo.Implementation
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly ApplicationDbContext _DBcontext;

        public CustomerRepo(ApplicationDbContext context)
        {
            _DBcontext = context;
        }

        // Add a customer
        public async Task<bool> AddAsync(Customer customer)
        {
            try
            {
                if (customer == null)
                    throw new ArgumentNullException(nameof(customer));

                await _DBcontext.Customers.AddAsync(customer);
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
                return false;
            }
        }

        // Soft delete a customer by ID (Code)
        public async Task<bool> DeleteAsync(string code)
        {
            try
            {
                var customer = await _DBcontext.Customers.FirstOrDefaultAsync(c => c.Code == code);
                if (customer == null)
                    return false;

                // For soft delete, assume there is an `IsDeleted` property.
                // Update this property if it exists, otherwise remove the record.
                _DBcontext.Customers.Remove(customer);
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }

        // Get all customers
        public async Task<List<Customer>> GetAllAsync()
        {
            try
            {
                // Add filter if soft-delete is implemented
                return await _DBcontext.Customers
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customers: {ex.Message}");
                return new List<Customer>();
            }
        }

        // Get customer by ID (Code)
        public async Task<Customer> GetByIdAsync(string code)
        {
            try
            {
                return await _DBcontext.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Code == code);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer by Code: {ex.Message}");
                return null;
            }
        }

        // Update an existing customer
        public async Task<bool> UpdateAsync(Customer customer)
        {
            try
            {
                if (customer == null)
                    throw new ArgumentNullException(nameof(customer));

                var existingCustomer = await _DBcontext.Customers
                    .FirstOrDefaultAsync(c => c.Code == customer.Code);

                if (existingCustomer == null)
                    return false;

                // Update the properties
                existingCustomer.Name = customer.Name;
                existingCustomer.Code = customer.Code;
                existingCustomer.Governate = customer.Governate;
                existingCustomer.City = customer.City;
                existingCustomer.PhoneNumber = customer.PhoneNumber;

                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // Check if a customer exists by ID (Code)
        public async Task<bool> ExistsAsync(string code)
        {
            try
            {
                return await _DBcontext.Customers.AnyAsync(c => c.Code == code);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking customer existence: {ex.Message}");
                return false;
            }
        }
    }
}
