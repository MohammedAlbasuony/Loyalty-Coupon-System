using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class ExchangeOrderController : Controller
    {
        private readonly IExchangeOrderService _service;
        private readonly ApplicationDbContext _context; 

        public ExchangeOrderController(IExchangeOrderService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }
        [Authorize(Policy = "Exchange Permissions")]
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
        public async Task<IActionResult> GetDistributorDetails(string technicianCodeOrName)
        {
            var model = await _service.GetDistributorDetailsAsync(technicianCodeOrName);
            return PartialView("_DistributorDetails", model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignQRCode()
        {
            var model = await _service.GetAssignmentDetailsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignQRCode(string selectedCustomerCode, string selectedDistributorCode, string selectedGovernate, string selectedCity, List<AssignmentViewModel> transactions)
        {
            var assignmentMapping = new AssignmentViewModel
            {
                SelectedCustomerCode = selectedCustomerCode,
                SelectedDistributorCode = selectedDistributorCode,
                SelectedGovernate = selectedGovernate,
                SelectedCity = selectedCity
            };

            ServiceToManageStatues serviceToManageStatues = new ServiceToManageStatues(_context);

            // Validate required fields
            if (string.IsNullOrEmpty(selectedCustomerCode))
            {
                ModelState.AddModelError("selectedCustomerCode", "Customer is required.");
            }
            if (string.IsNullOrEmpty(selectedDistributorCode))
            {
                ModelState.AddModelError("selectedDistributorCode", "Distributor is required.");
            }
            if (string.IsNullOrEmpty(selectedGovernate))
            {
                ModelState.AddModelError("selectedGovernate", "Governate is required.");
            }
            if (string.IsNullOrEmpty(selectedCity))
            {
                ModelState.AddModelError("selectedCity", "City is required.");
            }

            foreach (var transaction in transactions)
            {
                // Fetch the last recorded sequence number for the specific coupon type
                var lastSequence = _context.Transactions
                    .Where(t => t.CouponType == transaction.SelectedCouponType)
                    .OrderByDescending(t => t.TransactionID)
                    .Select(t => t.SequenceEnd)
                    .FirstOrDefault() ?? "0"; // Default to "0" if no records exist for that coupon type

                // Validate that SequenceStart is greater than the last recorded sequence for this coupon type
                if (string.Compare(transaction.SequenceStart, lastSequence) <= 0)
                {
                    ModelState.AddModelError(nameof(transaction.SequenceStart),
                        $"SequenceStart must be greater than the last recorded sequence number for coupon type {transaction.SelectedCouponType}: {lastSequence}");
                }

                // Update Status, Representative, Customer
                serviceToManageStatues.ManageStatuesEcxhangeOrder(
                    transaction.SequenceStart, transaction.SequenceEnd,
                    selectedCustomerCode, selectedDistributorCode);

                var couponIdentifier = GetCouponIdentifier(transaction.SelectedCouponType);

                // Validate that SequenceStart and SequenceEnd start with the correct prefix
                if (!transaction.SequenceStart.StartsWith(couponIdentifier) ||
                    !transaction.SequenceEnd.StartsWith(couponIdentifier))
                {
                    ModelState.AddModelError(string.Empty,
                        $"Start and End Sequences must start with '{couponIdentifier}' for the selected coupon type.");
                }

                // Ensure unique sequence range
                var isDuplicate = _context.Transactions.Any(t =>
                    t.CouponType == transaction.SelectedCouponType &&
                    string.Compare(t.SequenceStart, transaction.SequenceEnd) <= 0 &&
                    string.Compare(t.SequenceEnd, transaction.SequenceStart) >= 0);

                if (isDuplicate)
                {
                    ModelState.AddModelError(string.Empty,
                        $"The sequence range {transaction.SequenceStart}-{transaction.SequenceEnd} is already used for coupon type {transaction.SelectedCouponType}.");
                }

                // Check for duplicate Exchange Permission
                if (await _service.IsExchangePermissionDuplicateAsync(transaction.ExchangePermission))
                {
                    ModelState.AddModelError("ExchangePermission",
                        $"Exchange Permission Number {transaction.ExchangePermission} is already used.");
                }
            }

            // If there are validation errors, return to the view with the model
            if (!ModelState.IsValid)
            {
                // Return the assignmentMapping with selected values to the view
                return View(assignmentMapping);
            }

            // Call the service to assign QR codes after validation
            await _service.AssignQRCodeAsync(selectedCustomerCode, selectedDistributorCode, selectedGovernate, selectedCity, transactions);
            TempData["SuccessMessage"] = "All assignments saved successfully.";

            return RedirectToAction("AllTransactions", "Transaction");
        }

        // Helper method to map CouponType to the identifier
        private string GetCouponIdentifier(string couponType)
        {
            return couponType switch
            {
                "راك ثيرم" => "1",
                "صرف جي تكس" => "2",
                "اقطار كبيرة وهودذا" => "3",
                "كعب راك ثيرم" => "4",
                "كعب صرف جي تكس" => "5",
                "كعب اقطار كبيرة وهودذا" => "6",
                _ => throw new ArgumentException("Invalid coupon type"),
            };
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
