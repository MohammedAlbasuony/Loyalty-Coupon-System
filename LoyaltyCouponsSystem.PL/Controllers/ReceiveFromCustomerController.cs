using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.BLL.ViewModel.ReceiveFromCustomer;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class ReceiveFromCustomerController : Controller
    {
        private readonly ApplicationDbContext _DBcontext;

        public ReceiveFromCustomerController(ApplicationDbContext context)
        {
            _DBcontext = context;
        }

        public async Task<IActionResult> ReturnItems()
        {
            // Prepare ViewBag for dropdowns
            ViewBag.Customers = await _DBcontext.Customers
                .Where(c => c.IsActive)
                .Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    Name = c.Name,
                    Code = c.Code
                })
                .ToListAsync();

            ViewBag.Distributors = await _DBcontext.Distributors
                .Where(d => d.IsActive)
                .Select(d => new DistributorViewModel
                {
                    DistributorID = d.DistributorID,
                    Name = d.Name,
                    Code = d.Code
                })
                .ToListAsync();

            ViewBag.Technicians = await _DBcontext.Technicians
                .Where(t => t.IsActive)
                .Select(t => new TechnicianViewModel
                {
                    TechnicianID = t.TechnicianID,
                    Name = t.Name,
                    Code = t.Code
                })
                .ToListAsync();

            // Fetch Governorates only
            ViewBag.Governorates = await _DBcontext.Governorates
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();

            return View(new ReceiveFromCustomerViewModel());
        }
        [HttpGet("GetAreasByGovernorate/{governorateId}")]
        public IActionResult GetAreasByGovernorate(int governorateId)
        {
            var areas = _DBcontext.Areas
                .Where(a => a.GovernateId == governorateId)
                .Select(a => new { a.Id, a.Name })
                .ToList();

            if (!areas.Any())
            {
                return NotFound("No areas found for the selected governorate.");
            }

            return Ok(areas);
        }




        [HttpPost]
        public async Task<IActionResult> SubmitReturnItems(ReceiveFromCustomerViewModel model)
        {


            
            // Create and save the transaction
            try
            {
                // Create and save the transaction
                var transaction = new ReceiveFromCustomer
                {
                    CustomerCode = _DBcontext.Customers.Where(c => c.CustomerID == model.CustomerId).Select(c => c.Code).FirstOrDefault(),
                    DistributorCode = _DBcontext.Distributors.Where(d => d.DistributorID == model.DistributorId).Select(d => d.Code).FirstOrDefault(),
                    TechnicianCode = _DBcontext.Technicians.Where(t => t.TechnicianID == model.TechnicianId).Select(t => t.Code).FirstOrDefault(),
                    GovernorateId = model.GovernorateId,
                    Governorates = _DBcontext.Governorates.FirstOrDefault(g => g.Id == model.GovernorateId),
                    AreaId = model.CityId,
                    Areas = _DBcontext.Areas.FirstOrDefault(a => a.Id == model.CityId),
                    CouponReceiptNumber = model.CouponReceiptNumber,
                    TransactionDate = DateTime.Now
                };

                _DBcontext.ReceiveFromCustomers.Add(transaction);
                await _DBcontext.SaveChangesAsync();

                TempData["Success"] = "Transaction submitted successfully!";
            }
            catch (Exception ex)
            {
                var customerExists = _DBcontext.Customers.Any(c => c.CustomerID == model.CustomerId);
                if (!customerExists)
                {
                    TempData["Error"] = "Invalid CustomerId. Please select a valid customer.";
                    return RedirectToAction("ReturnItems");
                }
                var areaExists = _DBcontext.Areas.Any(a => a.Id == model.CityId);
                if (!areaExists)
                {
                    TempData["Error"] = "Invalid AreaId. Please select a valid area.";
                    return RedirectToAction("ReturnItems");
                }
                TempData["Error"] = "An error occurred while submitting the transaction. Please try again.";
            }

            return RedirectToAction("AllTransactions");
        }


        public async Task<IActionResult> AllTransactions(int page = 1, int pageSize = 10)
        {
            var totalTransactions = await _DBcontext.ReceiveFromCustomers
                .CountAsync();

            var transactions = await _DBcontext.ReceiveFromCustomers
                .Include(t => t.Governorates)  // Only include related entities
                .Include(t => t.Areas)
                .OrderByDescending(t => t.TransactionDate)  // Latest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new ReceiveFromCustomerViewModel
                {
                    CustomerCodeAndName = t.CustomerCode,  // No need to Include here
                    DistributorCodeAndName = t.DistributorCode,
                    TechnicianCodeAndName = t.TechnicianCode,
                    GovernorateName = t.Governorates.Name,
                    AreaName = t.Areas.Name,
                    CouponReceiptNumber = t.CouponReceiptNumber,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalTransactions / (double)pageSize);

            var model = new TransactionPaginationViewModel
            {
                Transactions = transactions,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }


    }
}