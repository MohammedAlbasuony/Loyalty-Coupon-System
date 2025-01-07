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

        //  add method
        public async Task<bool> AddAsync(Customer customer)
        {
            try
            {
                await _DBcontext.Customers.AddAsync(customer);
                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding doctor: {ex.Message}");
                return false;
            }
        }

        //  delete method
        public async Task<bool> DeleteAsync(string id)
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

        //  get all method
        public async Task<List<Customer>> GetAllAsync()
        {
            try
            {
                return await _DBcontext.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctors: {ex.Message}");
                return new List<Customer>();
            }
        }

        //  get by ID method
        public async Task<Customer> GetByIdAsync(string id)
        {
            try
            {
                return await _DBcontext.Customers.Where(p => p.Code == id).FirstOrDefaultAsync();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctor by ID: {ex.Message}");
                return null;
            }
        }

       


        //  update method
        public async Task<bool> UpdateAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _DBcontext.Customers
                    .Where(p => p.Code == customer.Code)
                    .FirstOrDefaultAsync();

                if (existingCustomer == null)
                {
                    return false;
                }

                // Update the properties
                existingCustomer.Name = customer.Name;
                existingCustomer.ContactDetails = customer.ContactDetails;

                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating doctor: {ex.Message}");
                return false;
            }
        }
    }
}

