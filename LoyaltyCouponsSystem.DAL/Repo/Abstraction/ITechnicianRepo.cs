using LoyaltyCouponsSystem.DAL.DB;
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
        Task<List<string>> GetValidUserIdsAsync(List<string> userNamesOrEmails);
        Task<List<int>> GetValidCustomerIdsAsync(List<string> customerCodes);
        Task<List<Customer>> GetCustomersForDropdownAsync();
        Task<List<ApplicationUser>> GetUsersForDropdownAsync();
    }
}
