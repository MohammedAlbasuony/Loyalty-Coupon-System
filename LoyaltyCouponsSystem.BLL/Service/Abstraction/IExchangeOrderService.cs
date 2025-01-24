using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface IExchangeOrderService
    {
        Task<CustomerViewModel> GetCustomerDetailsAsync(string customerCodeOrName);
        Task<TechnicianViewModel> GetTechnicianDetailsAsync(string technicianCodeOrName);
        Task<AssignmentViewModel> GetAssignmentDetailsAsync();
        Task AssignQRCodeAsync(string selectedCustomerCode, string selectedTechnicianCode, string selectedGovernate, string selectedCity, List<AssignmentViewModel> transactions);

        // New method for checking duplicate Exchange Permission Number
        Task<bool> IsExchangePermissionDuplicateAsync(string exchangePermission);
    }
}
