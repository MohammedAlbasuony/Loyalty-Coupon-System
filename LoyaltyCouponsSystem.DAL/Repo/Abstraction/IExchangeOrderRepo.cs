using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
