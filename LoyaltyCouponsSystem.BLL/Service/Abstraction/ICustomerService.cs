using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface ICustomerService
    {
        Task<bool> AddAsync(CustomerViewModel customerViewModel);
        Task<bool> DeleteAsync(string id);
        Task<List<CustomerViewModel>> GetAllAsync();
        Task<CustomerViewModel> GetByIdAsync(string id);
        Task<bool> UpdateAsync(UpdateCustomerViewModel updateCustomerViewModel);
    }
}
