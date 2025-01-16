using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface ITechnicianService
    {
        Task<bool> AddAsync(TechnicianViewModel technicianViewModel);
        Task<bool> DeleteAsync(string id);
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<TechnicianViewModel> GetByIdAsync(string id);
        Task<bool> UpdateAsync(TechnicianViewModel technicianViewModel);
        Task<List<SelectListItem>> GetCustomersForDropdownAsync();
        Task<List<SelectListItem>> GetUsersForDropdownAsync();
    }
}
