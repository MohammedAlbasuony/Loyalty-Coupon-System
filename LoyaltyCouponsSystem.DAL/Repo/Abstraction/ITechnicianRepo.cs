using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface ITechnicianRepo
    {
        Task<bool> AddAsync(Technician technician);
        Task<bool> DeleteAsync(int id);
        Task<List<Technician>> GetAllAsync();
        Task<Technician> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Technician technician);
        Task<List<string>> GetValidUserIdsAsync(List<string> userNamesOrEmails);
        Task<List<int>> GetValidCustomerIdsAsync(List<string> customerCodes);
        Task<List<Customer>> GetCustomersForDropdownAsync();
        Task<List<ApplicationUser>> GetUsersForDropdownAsync();
        Task AssignCustomerAsync(int technicianId, int customerId);
        Task RemoveCustomerByNameAsync(int technicianId, string customerName);
        Task<List<Customer>> GetActiveUnassignedCustomersAsync();
        Task AssignRepresentativeAsync(int technicianId, string userId);
        Task RemoveRepresentativeAsync(int technicianId, string userName);
        Task<List<ApplicationUser>> GetActiveUnassignedRepresentativesAsync();
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    }
}
