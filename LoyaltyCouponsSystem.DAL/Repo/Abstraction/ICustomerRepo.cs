using LoyaltyCouponsSystem.DAL.Entity;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface ICustomerRepo
    {
        Task<bool> AddAsync(Customer customer);
        Task<bool> DeleteAsync(string id);
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(string id);
        Task<bool> UpdateAsync(Customer customer);
        Task<List<int>> GetCustomerIdsByCodesAsync(List<string> customerCodes);
        Task<bool> IsUniqueCode(string code);
        Task<bool> IsUniquePhoneNumber(string phoneNumber);
        Task<List<Customer>> GetCustomersByIdsAsync(List<int> customerIds);
    }

}
