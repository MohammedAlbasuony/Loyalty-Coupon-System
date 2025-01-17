using LoyaltyCouponsSystem.DAL.Entity;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface IDistributorRepo
    {
        Task<bool> AddAsync(Distributor distributor);
        Task<bool> DeleteAsync(int id);
        Task<List<Distributor>> GetAllAsync();
        Task<Distributor> GetByCodeAsync(string code);
        Task<bool> UpdateAsync(Distributor distributor);
        Task<List<Customer>> GetCustomersForDropdownAsync();
        Task<bool> IsUniquePhoneNumberAsync(int phoneNumber);
        Task<bool> IsUniqueCodeAsync(string code);

    }
}
