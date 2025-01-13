using LoyaltyCouponsSystem.DAL.Entity;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface IDistributorRepo
    {
        Task<bool> AddAsync(Distributor distributor);
        Task<bool> DeleteAsync(int id);
        Task<List<Distributor>> GetAllAsync();
        Task<Distributor> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Distributor distributor);
        Task<List<Customer>> GetCustomersForDropdownAsync();
        Task<List<int>> GetValidCustomerIdsAsync(List<string> customerCodes);
    }
}
