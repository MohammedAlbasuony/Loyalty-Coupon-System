using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            // Fetch only active customers, distributors, and technicians
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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReturnItems(string CustomerCode, string DistributorCode, string TechnicianCode, string Governorate, string City, string CouponReceiptNumber)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(CustomerCode) || string.IsNullOrEmpty(DistributorCode) || string.IsNullOrEmpty(TechnicianCode) ||
                string.IsNullOrEmpty(Governorate) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(CouponReceiptNumber))
            {
                TempData["Error"] = "All fields are required!";
                return RedirectToAction("ReturnItems");
            }

            try
            {
                // Parse the inputs and create a new transaction
                var transaction = new ReceiveFromCustomer
                {
                    CustomerCode = CustomerCode,
                    DistributorCode = DistributorCode,
                    TechnicianCode = TechnicianCode,
                    Governorate = Governorate,
                    City = City,
                    CouponReceiptNumber = long.Parse(CouponReceiptNumber), // Still numeric
                    TransactionDate = DateTime.Now
                };

                // Save transaction to the database
                _DBcontext.ReceiveFromCustomers.Add(transaction);
                await _DBcontext.SaveChangesAsync();

                // Success message
                TempData["Success"] = "Transaction submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }

            return RedirectToAction("ReturnItems");
        }






        // AllTransactions Action
        public IActionResult AllTransactions()
        {
            return View(); // Create a view for this action (e.g., Views/ReceiveFromCustomer/AllTransactions.cshtml)
        }
    }
}
