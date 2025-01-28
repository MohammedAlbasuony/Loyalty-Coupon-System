using LoyaltyCouponsSystem.DAL.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Repo.Abstraction
{
    public interface IExchangeOrderRepo
    {
        Task<Customer> GetCustomerByCodeOrNameAsync(string customerCodeOrName);
        Task<Technician> GetTechnicianByCodeOrNameAsync(string technicianCodeOrName);
        Task<List<Customer>> GetAllCustomersAsync();
        Task<List<Technician>> GetAllTechniciansAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task SaveChangesAsync();
        Task<bool> TransactionExistsAsync(string exchangePermission, long sequenceNumber);
        Task<Distributor> GetDistributorByCodeOrNameAsync(string technicianCodeOrName);
        Task<List<Distributor>> GetAllDistributorsAsync();

        // New method to check for duplicate Exchange Permission Number
        Task<bool> ExchangePermissionExistsAsync(string exchangePermission);
    }
}
