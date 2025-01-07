using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer_and_Technician;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class ExchangeOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExchangeOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get customer details based on code or name search
        public IActionResult GetCustomerDetails(string customerCodeOrName)
        {
            var customer = _context.Customers
                .FirstOrDefault(c => c.Code == customerCodeOrName || c.Name == customerCodeOrName);

            if (customer == null)
            {
                return PartialView("_CustomerDetails", null);
            }

            var model = new CustomerViewModel
            {
                Code = customer.Code,
                Name = customer.Name
            };

            return PartialView("_CustomerDetails", model);
        }

        // Get technician details based on code or name search
        public IActionResult GetTechnicianDetails(string technicianCodeOrName)
        {
            var technician = _context.Technicians
                .FirstOrDefault(t => t.Code == technicianCodeOrName || t.Name == technicianCodeOrName);

            if (technician == null)
            {
                return PartialView("_TechnicianDetails", null);
            }

            var model = new TechnicianViewModel
            {
                Code = technician.Code,
                Name = technician.Name
            };

            return PartialView("_TechnicianDetails", model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignQRCode()
        {
            // Fetch all customers, technicians, governates
            var customers = await _context.Customers.ToListAsync();
            var technicians = await _context.Technicians.ToListAsync();

            // Create and populate the model for dropdowns
            var model = new AssignmentViewModel
            {
                Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Code,
                    Text = $"{c.Name} ({c.Code})"
                }).ToList(),
                Technicians = technicians.Select(t => new SelectListItem
                {
                    Value = t.Code,
                    Text = $"{t.Name} ({t.Code})"
                }).ToList(),

                Governates = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Cairo", Text = "Cairo" },
                    new SelectListItem { Value = "Gharbeia", Text = "Gharbeia" },
                    new SelectListItem { Value = "Sharqeia", Text = "Sharqeia" },
                    new SelectListItem { Value = "Alexandria", Text = "Alexandria" }
                },

                CouponSorts = new List<SelectListItem>
                {
                    new SelectListItem { Value = "اختبار", Text = "كعب إختبار" },
                    new SelectListItem { Value = "عادي", Text = "كوبون عادي" },
                    new SelectListItem { Value = "حافز", Text = "كوبون حافز" },
                    new SelectListItem { Value = "مرتجع", Text = "كوبون مرتجع" }
                },

                CouponTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "راك ثيرم", Text = "راك ثيرم" },
                    new SelectListItem { Value = "صرف جي تكس", Text = "صرف جي تكس" },
                    new SelectListItem { Value = "اقطار كبيرة وهودذا", Text = "اقطار كبيرة وهودذا" },
                    new SelectListItem { Value = "كعب راك ثيرم", Text = "كعب راك ثيرم" },
                    new SelectListItem { Value = "كعب صرف جي تكس", Text = "كعب صرف جي تكس" },
                    new SelectListItem { Value = "كعب اقطار كبيرة وهودذا", Text = "كعب اقطار كبيرة وهودذا" }
                }
            };

            return View(model);
        }

        // AJAX GET action to load cities based on the selected governate
        [HttpGet]
        public JsonResult GetCitiesByGovernate(string governate)
        {
            // Simulate the cities based on the selected governate (replace with actual logic)
            var cities = governate switch
            {
                "Cairo" => new List<SelectListItem>
                {
                    new SelectListItem { Value = "CairoCity1", Text = "Cairo City 1" },
                    new SelectListItem { Value = "CairoCity2", Text = "Cairo City 2" }
                },
                "Gharbeia" => new List<SelectListItem>
                {
                    new SelectListItem { Value = "GharbeiaCity1", Text = "Gharbeia City 1" },
                    new SelectListItem { Value = "GharbeiaCity2", Text = "Gharbeia City 2" }
                },
                _ => new List<SelectListItem>()
            };

            return Json(cities);
        }

        [HttpPost]
        public async Task<IActionResult> AssignQRCode(
            string SelectedCustomerCode,
            string SelectedTechnicianCode,
            string SelectedGovernate,
            string SelectedCity,
            List<AssignmentViewModel> Transactions)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Code == SelectedCustomerCode);
            var technician = await _context.Technicians.FirstOrDefaultAsync(t => t.Code == SelectedTechnicianCode);

            if (customer != null && technician != null && Transactions != null)
            {
                foreach (var transaction in Transactions)
                {
                    for (int seqNum = transaction.StartSequenceNumber; seqNum <= transaction.EndSequenceNumber; seqNum++)
                    {
                        var newTransaction = new Transaction
                        {
                            CustomerID = customer.CustomerID,
                            TechnicianID = technician.TechnicianID,
                            Governate = SelectedGovernate,
                            City = SelectedCity,
                            Timestamp = DateTime.Now,
                            CouponSort = transaction.SelectedCouponSort,
                            CouponType = transaction.SelectedCouponType,
                            SequenceNumber = seqNum,
                            ExchangePermission = transaction.ExchangePermission
                        };

                        _context.Transactions.Add(newTransaction);
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "All assignments saved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to find customer, technician, or transactions. Please check the input.";
            }

            return RedirectToAction(nameof(AssignQRCode));
        }
    }
}
