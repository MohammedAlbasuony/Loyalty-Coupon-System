using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.DAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.DAL.Repo.Implementation
{
    public class TechnicianRepo : ITechnicianRepo
    {
        private readonly ApplicationDbContext _DBcontext;

        public TechnicianRepo(ApplicationDbContext context)
        {
            _DBcontext = context;
        }

        //  add method
        public async Task<bool> AddAsync(Technician technician)
        {
            try
            {
                await _DBcontext.Technicians.AddAsync(technician);
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
        public async Task<List<Technician>> GetAllAsync()
        {
            try
            {
                return await _DBcontext.Technicians.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctors: {ex.Message}");
                return new List<Technician>();
            }
        }

        //  get by ID method
        public async Task<Technician> GetByIdAsync(string id)
        {
            try
            {
                return await _DBcontext.Technicians.Where(p => p.Code == id).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctor by ID: {ex.Message}");
                return null;
            }
        }

        //  update method
        public async Task<bool> UpdateAsync(Technician technician)
        {
            try
            {
                var existingTechnician = await _DBcontext.Customers
                    .Where(p => p.Code == technician.Code)
                    .FirstOrDefaultAsync();

                if (existingTechnician == null)
                {
                    return false;
                }

                // Update the properties
                existingTechnician.Name = technician.Name;
                existingTechnician.ContactDetails = technician.ContactDetails;

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
