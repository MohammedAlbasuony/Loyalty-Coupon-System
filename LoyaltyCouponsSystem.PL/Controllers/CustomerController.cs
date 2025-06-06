using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Customer;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _DBcontext;

        public CustomerController(ApplicationDbContext context, ICustomerService customerService)
        {
            _DBcontext = context;
            _customerService = customerService;
        }
        [Authorize(Policy = "Manage Customers")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllAsync();
            return View(result);
        }

        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return View(result);
        }


        [HttpPost]
        [Route("Customer/ToggleActivation")]
        public async Task<IActionResult> ToggleActivation(int customerId)
        {
            var customer = await _DBcontext.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return Json(new { success = false, message = "Customer not found" });
            }

            // Toggle the IsActive status
            customer.IsActive = !customer.IsActive;
            await _DBcontext.SaveChangesAsync();

            // Return the updated status
            return Json(new { success = true, isActive = customer.IsActive });
        }







        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.AddAsync(customerViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllCustomers");
                }
                ModelState.AddModelError("", "Unable to add customer. Please try again.");
            }

            return View(customerViewModel);
        }

        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customerService.DeleteAsync(id);
            return RedirectToAction(nameof(GetAllCustomers));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var updateCustomerViewModel = new UpdateCustomerViewModel
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Code = customer.Code,
                Governate = customer.Governate,
                City = customer.City,
                PhoneNumber = customer.PhoneNumber,
                TechnicianID = customer.TechnicianID,
                IsActive = customer.IsActive // Assuming IsActive is part of the ApplicationUser class

            };

            return View(updateCustomerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerViewModel updateCustomerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateAsync(updateCustomerViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllCustomers");
                }
                ModelState.AddModelError("", "Failed to update customer");
            }

            return View(updateCustomerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction("GetAllCustomers");
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                // Process the file in the service
                var result = await _customerService.ImportCustomersFromExcelAsync(stream);

                if (result)
                {
                    TempData["SuccessMessage"] = "Customers imported successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to import customers. Please check the file format.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("GetAllCustomers");
        }

    }
}
