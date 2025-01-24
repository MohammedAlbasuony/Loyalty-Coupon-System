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
            // Server-side validation for Exchange Permission Number uniqueness
            foreach (var transaction in transactions)
            {
                if (await _service.IsExchangePermissionDuplicateAsync(transaction.ExchangePermission))
                {
                    ModelState.AddModelError("ExchangePermission", $"Exchange Permission Number {transaction.ExchangePermission} is already used.");
                    var model = await _service.GetAssignmentDetailsAsync(); // Reload the model in case of validation errors
                    return View(model);
                }
            }

            // Proceed with saving if validation passes
            await _service.AssignQRCodeAsync(selectedCustomerCode, selectedTechnicianCode, selectedGovernate, selectedCity, transactions);
            TempData["SuccessMessage"] = "All assignments saved successfully.";
            return RedirectToAction("AllTransactions", "Transaction");
        }

        // New action for AJAX validation
        [Route("ExchangeOrder/CheckExchangePermission")]
        [HttpPost]
        public async Task<JsonResult> CheckExchangePermission(string exchangePermission)
        {
            try
            {
                var isDuplicate = await _service.IsExchangePermissionDuplicateAsync(exchangePermission);
                return Json(isDuplicate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error for debugging
                return Json(false); // Return false to indicate an issue
            }
        }

    }
}
