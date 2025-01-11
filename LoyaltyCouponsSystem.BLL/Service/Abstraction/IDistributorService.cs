using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface IDistributorService
    {
        Task<bool> AddAsync(DistributorViewModel distributorViewModel);
        Task<bool> DeleteAsync(int id);
        Task<List<DistributorViewModel>> GetAllAsync();
        Task<DistributorViewModel> GetByIdAsync(int id);
        Task<bool> UpdateAsync(DistributorViewModel distributorViewModel);
        Task<List<SelectListItem>> GetCustomersForDropdownAsync(); 
    }
}
