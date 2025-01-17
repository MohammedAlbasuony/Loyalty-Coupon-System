using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class ExchangeOrderController : Controller
    {
        private readonly IExchangeOrderService _service;

        public ExchangeOrderController(IExchangeOrderService service)
        {
            _service = service;
        }
        
        public async Task<IActionResult> GetCustomerDetails(string customerCodeOrName)
        {
            var model = await _service.GetCustomerDetailsAsync(customerCodeOrName);
            return PartialView("_CustomerDetails", model);
        }

        public async Task<IActionResult> GetTechnicianDetails(string technicianCodeOrName)
        {
            var model = await _service.GetTechnicianDetailsAsync(technicianCodeOrName);
            return PartialView("_TechnicianDetails", model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignQRCode()
        {
            var model = await _service.GetAssignmentDetailsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignQRCode(string selectedCustomerCode, string selectedTechnicianCode, string selectedGovernate, string selectedCity, List<AssignmentViewModel> transactions)
        {
            await _service.AssignQRCodeAsync(selectedCustomerCode, selectedTechnicianCode, selectedGovernate, selectedCity, transactions);
            TempData["SuccessMessage"] = "All assignments saved successfully.";
            return RedirectToAction("AllTransactions", "Transaction");
        }
    }
}
