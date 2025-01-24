using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface ITechnicianService
    {
        Task<bool> AddAsync(TechnicianViewModel technicianViewModel);
        Task<bool> DeleteAsync(int id);
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<UpdateTechnicianViewModel> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UpdateTechnicianViewModel TechnicianViewModel);
        Task<List<SelectListItem>> GetCustomersForDropdownAsync();
        Task<List<SelectListItem>> GetUsersForDropdownAsync();
        Task<bool> ToggleActivationAsync(int distributorId);
        Task<bool> ImportTechniciansFromExcelAsync(Stream stream);
    }
}
