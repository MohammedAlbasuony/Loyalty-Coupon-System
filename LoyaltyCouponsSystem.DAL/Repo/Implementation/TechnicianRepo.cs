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

        // Add method
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
                Console.WriteLine($"Error adding technician: {ex.Message}");
                return false;
            }
        }

        // Delete method
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var technician = await _DBcontext.Technicians.Where(t => t.Code == id).FirstOrDefaultAsync();
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
                return await _DBcontext.Technicians.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching technicians: {ex.Message}");
                return new List<Technician>();
            }
        }

        // Get by ID method
        public async Task<Technician> GetByIdAsync(string id)
        {
            try
            {
                return await _DBcontext.Technicians.Where(t => t.Code == id).FirstOrDefaultAsync();
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
                var existingTechnician = await _DBcontext.Technicians
                    .Where(t => t.Code == technician.Code)
                    .FirstOrDefaultAsync();

                if (existingTechnician == null)
                {
                    return false;
                }

                // Update the properties
                existingTechnician.Name = technician.Name;
                existingTechnician.NickName = technician.NickName;
                existingTechnician.NationalID = technician.NationalID;
                existingTechnician.PhoneNumber1 = technician.PhoneNumber1;
                existingTechnician.PhoneNumber2 = technician.PhoneNumber2;
                existingTechnician.PhoneNumber3 = technician.PhoneNumber3;
                existingTechnician.Governate = technician.Governate;
                existingTechnician.City = technician.City;

                await _DBcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating technician: {ex.Message}");
                return false;
            }
        }
    }
}
