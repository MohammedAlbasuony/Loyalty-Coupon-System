using LoyaltyCouponsSystem.BLL.ViewModel.Technician;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface ITechnicianService
    {
        Task<bool> AddAsync(TechnicianViewModel technicianViewModel);
        Task<bool> DeleteAsync(string id);
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<TechnicianViewModel> GetByIdAsync(string id);
        Task<bool> UpdateAsync(TechnicianViewModel technicianViewModel);
    }
}
