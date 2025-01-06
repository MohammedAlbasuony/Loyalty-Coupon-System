using LoyaltyCouponsSystem.DAL.Entity;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface ITechnicianRepo
    {
        Task<bool> AddAsync(Technician technician);
        Task<bool> DeleteAsync(string id);
        Task<List<Technician>> GetAllAsync();
        Task<Technician> GetByIdAsync(string id);
        Task<bool> UpdateAsync(Technician technician);
    }
}
